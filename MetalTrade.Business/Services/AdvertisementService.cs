using AutoMapper;
﻿using MetalTrade.Application.Patterns.StateMachine.Advertisement;
using MetalTrade.Business.Dtos;
using MetalTrade.Business.Interfaces;
using MetalTrade.DataAccess.Data;
using MetalTrade.DataAccess.Repositories;
using MetalTrade.Domain.Entities;
using MetalTrade.Domain.Enums;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace MetalTrade.Business.Services;

public class AdvertisementService : IAdvertisementService
{
    private readonly AdvertisementRepository _repository;
    private readonly IMapper _mapper;
    private readonly IImageUploadService _imageUploadService;
    private AdvertisementStateContext _stateContext;
    private readonly IPromotionService _promotionService;

    public AdvertisementService(
        MetalTradeDbContext context, 
        IMapper mapper, 
        IImageUploadService imageUploadService,
        IPromotionService promotionService)
    {
        _repository = new AdvertisementRepository(context);
        _mapper = mapper;
        _imageUploadService = imageUploadService;
        _stateContext = new AdvertisementStateContext(_repository);
        _promotionService = promotionService;
    }

    public async Task<List<AdvertisementDto>> GetAllAsync()
    {
        var ads = await _repository.GetAllAsync();
        return _mapper.Map<List<AdvertisementDto>>(ads);
    }

    public async Task<AdvertisementDto?> GetAsync(int advertisementId)
    {
        var ad = await _repository.GetAsync(advertisementId);
        if (ad == null)
            return null;

        await _promotionService.UpdatePromotionAsync(ad.Id);
        await _promotionService.SaveAllChangesAsync();

        var dto = _mapper.Map<AdvertisementDto>(ad);
        return dto;
    }

    public async Task CreateAsync(AdvertisementDto adsDto)
    {
        adsDto.Status = (int)AdvertisementStatus.Draft;

        var entity = _mapper.Map<Advertisement>(adsDto);

        if (adsDto.PhotoFiles != null && adsDto.PhotoFiles.Count > 0)
        {
            var photoLinks = await _imageUploadService.UploadImagesAsync(adsDto.PhotoFiles, "advertisement");
            foreach (var link in photoLinks)
            {
                entity.Photoes.Add(new AdvertisementPhoto { PhotoLink = link });
            }
        }

        await _repository.CreateAsync(entity);
        await _repository.SaveChangesAsync();
    }

    public async Task UpdateAsync(AdvertisementDto adsDto)
    {
        var entity = await _repository.GetAsync(adsDto.Id) ?? throw new ArgumentException("Объявление не найдено");

        _mapper.Map(adsDto, entity);
        await _repository.UpdateAsync(entity);
        await _repository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int advertisementId)
    {
        await _promotionService.DeactivatePromotionAsync(advertisementId);
        await _promotionService.SaveAllChangesAsync();

        await _stateContext.MoveToDeletedAsync(advertisementId);
        await _repository.DeleteAsync(advertisementId);
        await _repository.SaveChangesAsync();
        var entity = await _repository.GetAsync(advertisementId);
        if (entity != null)
        {
            foreach (var photo in entity.Photoes)
            {
                await _imageUploadService.DeleteImageAsync(photo.PhotoLink);
            }
        }
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

    public async Task<bool> DeleteAdvertisementPhotoAsync(AdvertisementPhotoDto advertisementPhoto)
    {
        await _repository.DeleteAdvertisementPhotoAsync(advertisementPhoto.Id);
        await _repository.SaveChangesAsync();
        var deleted = await _repository.GetAdvertisementPhotoAsync(advertisementPhoto.Id) is null;
        if (deleted)
            await _imageUploadService.DeleteImageAsync(advertisementPhoto.PhotoLink);
        return deleted;
    }
    
    public async Task<List<AdvertisementDto>> GetFilteredAsync(AdvertisementFilterDto filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        var queriableAdvertisements = _repository.CreateFilter();
        if (!string.IsNullOrWhiteSpace(filter.Title))
        {
            if (int.TryParse(filter.Title, out var adId))
            {
                queriableAdvertisements = queriableAdvertisements
                    .Where(a => a.Id == adId);
            }
            else
            {
                queriableAdvertisements = _repository
                    .FilterTitle(queriableAdvertisements, filter.Title);
            }
        }
        
        if (!string.IsNullOrWhiteSpace(filter.City))
            queriableAdvertisements = _repository.FilterCity(queriableAdvertisements, filter.City);
        
        if (filter.MetalTypeId.HasValue)
            queriableAdvertisements = _repository.FilterMetalType(queriableAdvertisements, filter.MetalTypeId.Value);

        if (filter.PriceFrom.HasValue)
            queriableAdvertisements = _repository.FilterPriceFrom(queriableAdvertisements, filter.PriceFrom.Value);

        if (filter.PriceTo.HasValue)
            queriableAdvertisements = _repository.FilterPriceTo(queriableAdvertisements, filter.PriceTo.Value);

        if (filter.DateFromUtc.HasValue)
            queriableAdvertisements = _repository.FilterDateFromUtc(queriableAdvertisements, filter.DateFromUtc.Value);

        if (filter.DateToUtc.HasValue)
            queriableAdvertisements = _repository.FilterDateToUtc(queriableAdvertisements, filter.DateToUtc.Value);
        
        if (filter.ProductId.HasValue)
            queriableAdvertisements = _repository.FilterProduct(queriableAdvertisements, filter.ProductId.Value);

        queriableAdvertisements = filter?.Sort switch
        {
            "price_asc" => queriableAdvertisements.OrderBy(a => a.Price),
            "price_desc" => queriableAdvertisements.OrderByDescending(a => a.Price),
            "date_asc" => queriableAdvertisements.OrderBy(a => a.CreateDate),
            "date_desc" => queriableAdvertisements.OrderByDescending(a => a.CreateDate),
            _ => queriableAdvertisements.OrderByDescending(a => a.CreateDate)
        };

        queriableAdvertisements = queriableAdvertisements.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize);

        var ads = await queriableAdvertisements.ToListAsync();
        return _mapper.Map<List<AdvertisementDto>>(ads);
    }

    public async Task<List<AdvertisementPhotoAjaxDto>?> CreateAdvertisementPhotoAsync(AdvertisementDto adsDto)
    {
        var entity = await _repository.GetAsync(adsDto.Id) 
            ?? throw new ArgumentException("Объявление не найдено");

        if (adsDto.PhotoFiles?.Any() != true) return null;

        var newPhotoEntities = new List<AdvertisementPhoto>();
        var photoLinks = await _imageUploadService.UploadImagesAsync(adsDto.PhotoFiles, "advertisement");
        newPhotoEntities.AddRange(photoLinks.Select(link => new AdvertisementPhoto { PhotoLink = link }));

        entity.Photoes.AddRange(newPhotoEntities);
        await _repository.UpdateAsync(entity);
        await _repository.SaveChangesAsync();

        var photoes = _mapper.Map<List<AdvertisementPhotoAjaxDto>>(newPhotoEntities);
        return photoes;
    }

    public async Task CreateCommercialAsync(CommercialDto commercialDto)
    {
        var commercial = _mapper.Map<Commercial>(commercialDto);
        await _promotionService.CreateCommercialAdvertisementAsync(commercial);
        await _promotionService.SaveAllChangesAsync();
    }

    public async Task CreateTopAdvertisementAsync(TopAdvertisementDto topAdvertisementDto)
    {
        var topAdvertisement = _mapper.Map<TopAdvertisement>(topAdvertisementDto);
        await _promotionService.CreateTopAdvertisementAsync(topAdvertisement);
        await _promotionService.SaveAllChangesAsync();
    }

    public async Task<List<AdvertisementDto>> GetAllCommercialsAsync()
    {
        var commercials = await _promotionService.GetAllActiveCommercialsAsync();
        return _mapper.Map<List<AdvertisementDto>>(commercials);
    }

    public async Task<List<AdvertisementDto>> GetAllActiveTopAdvertisementsAsync()
    {
        var topAdvertisements = await _promotionService.GetAllActiveTopAdvertisementsAsync();
        return _mapper.Map<List<AdvertisementDto>>(topAdvertisements);
    }

    public async Task DeactivatePromotionAsync(int advertisementId)
    {
        await _promotionService.DeactivatePromotionAsync(advertisementId);
        await _promotionService.SaveAllChangesAsync();
    }
}