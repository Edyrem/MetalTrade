using AutoMapper;
using MetalTrade.Business.Dtos;
using MetalTrade.Business.Interfaces;
using MetalTrade.DataAccess.Data;
using MetalTrade.DataAccess.Repositories;
using MetalTrade.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace MetalTrade.Business.Services
{
    public class AdvertisementService : IAdvertisementService
    {
        private readonly AdvertisementRepository _repository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public AdvertisementService(MetalTradeDbContext context, IMapper mapper, IWebHostEnvironment env )
        {
            _repository = new AdvertisementRepository(context);
            _mapper = mapper;
            _env = env;
        }

        public async Task<List<AdvertisementDto>> GetAllAsync()
        {
            var ads = await _repository.GetAllAsync();
            return _mapper.Map<List<AdvertisementDto>>(ads);
        }

        public async Task<AdvertisementDto?> GetAsync(int advertisementId)
        {
            var ads = await _repository.GetAsync(advertisementId);
            return _mapper.Map<AdvertisementDto>(ads);
        }

        public async Task CreateAsync(AdvertisementDto adsDto)
        {
            adsDto.Product = null;
            adsDto.User = null;

            var entity = _mapper.Map<Advertisement>(adsDto);

            entity.CreateDate = DateTime.UtcNow;

            if (adsDto.Photoes != null && adsDto.Photoes.Any())
            {
                entity.Photoes = adsDto.Photoes.Select(photoDto => new AdvertisementPhoto
                {
                    PhotoLink = photoDto.PhotoLink,
                }).ToList();
            }

            await _repository.CreateAsync(entity);
            await _repository.SaveChangesAsync();
        }

        public async Task UpdateAsync(AdvertisementDto adsDto)
        {
            var entity = await _repository.GetAsync(adsDto.Id);
            if (entity == null) throw new ArgumentException("Объявление не найдено");

            // Очищаем связанные сущности
            adsDto.Product = null;
            adsDto.User = null;

            // Маппим основные поля
            _mapper.Map(adsDto, entity);

            // Обрабатываем новые фото если есть
            if (adsDto.PhotoFiles != null && adsDto.PhotoFiles.Any())
            {
                // Сохраняем новые фото
                foreach (var photoFile in adsDto.PhotoFiles)
                {
                    var fileName = $"{Guid.NewGuid()}_{photoFile.FileName}";
                    var uploads = Path.Combine(_env.WebRootPath, "uploads", "ads");
                    var filePath = Path.Combine(uploads, fileName);

                    Directory.CreateDirectory(uploads);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await photoFile.CopyToAsync(stream);
                    }

                    // Добавляем новое фото
                    entity.Photoes ??= new List<AdvertisementPhoto>();
                    entity.Photoes.Add(new AdvertisementPhoto
                    {
                        PhotoLink = $"/uploads/ads/{fileName}"
                    });
                }
            }
            else if (adsDto.Photoes?.Any() ?? false)
            {
                entity.Photoes = adsDto.Photoes.Select(p => new AdvertisementPhoto
                {
                    PhotoLink = p.PhotoLink
                }).ToList();
            }

            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int advertisementId)
        {
            await _repository.DeleteAsync(advertisementId);
            await _repository.SaveChangesAsync();
        }

    }
}
