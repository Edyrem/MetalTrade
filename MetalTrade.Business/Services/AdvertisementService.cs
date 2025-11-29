using AutoMapper;
﻿using MetalTrade.Application.Patterns.StateMachine.Advertisement;
using MetalTrade.Business.Dtos;
using MetalTrade.Business.Interfaces;
using MetalTrade.DataAccess.Data;
using MetalTrade.DataAccess.Repositories;
using MetalTrade.Domain.Entities;
using MetalTrade.Domain.Enums;
using System.Linq.Expressions;

namespace MetalTrade.Business.Services;

public class AdvertisementService : IAdvertisementService
{
    private readonly AdvertisementRepository _repository;
    private readonly IMapper _mapper;
    private readonly IImageUploadService _imageUploadService;
    private AdvertisementStateContext _stateContext;

    public AdvertisementService(MetalTradeDbContext context, IMapper mapper, IImageUploadService imageUploadService)
    {
        _repository = new AdvertisementRepository(context);
        _mapper = mapper;
        _imageUploadService = imageUploadService;
        _stateContext = new AdvertisementStateContext(_repository);
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
        //adsDto.Product = null;
        //adsDto.User = null;
        adsDto.Status = (int)AdvertisementStatus.Draft;

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

        //adsDto.Product = null;
        //adsDto.User = null;

        _mapper.Map(adsDto, entity);

        if (adsDto.PhotoFiles != null && adsDto.PhotoFiles.Any())
        {
            foreach (var photoFile in adsDto.PhotoFiles)
            {
                var defaultFolder = Path.Combine("uploads", "ads");
                var photoLink = await _imageUploadService.UploadImageAsync(photoFile, defaultFolder);
                entity.Photoes.Add(new AdvertisementPhoto
                {
                    PhotoLink = photoLink
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
        await _stateContext.MoveToDeletedAsync(advertisementId);
        await _repository.DeleteAsync(advertisementId);
        await _repository.SaveChangesAsync();
    }

    public async Task ApproveAsync(int advertisementId)
    {
        await _stateContext.MoveToActiveAsync(advertisementId);
        await _repository.SaveChangesAsync();
    }

    public async Task RejectAsync(int advertisementId)
    {
        await _stateContext.MoveToRejectedAsync(advertisementId);
        await _repository.SaveChangesAsync();
    }

    public async Task ArchiveAsync(int advertisementId)
    {
        await _stateContext.MoveToArchivedAsync(advertisementId);
        await _repository.SaveChangesAsync();
    }

    public async Task<IEnumerable<AdvertisementDto>> FindAsync(Expression<Func<Advertisement, bool>> predicate)
    {
        return _mapper.Map<List<AdvertisementDto>>(await _repository.FindAsync(predicate));
    }
}