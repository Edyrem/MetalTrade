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

        if (adsDto.PhotoFiles != null && adsDto.PhotoFiles.Count > 0)
        {
            var photoLinks = await _imageUploadService.UploadImagesAsync(adsDto.PhotoFiles, "advertisement");
            foreach (var link in photoLinks)
            {
                entity.Photoes.Add(new AdvertisementPhoto 
                { 
                    PhotoLink = link,
                    AdvertisementId = entity.Id
                });
            }
        }

        await _repository.UpdateAsync(entity);
        await _repository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int advertisementId)
    {
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

    public async Task DeleteAdvertisementPhotoAsync(AdvertisementPhotoDto advertisementPhoto)
    {
        await _imageUploadService.DeleteImageAsync(advertisementPhoto.PhotoLink);
        await _repository.DeleteAdvertisementPhotoAsync(advertisementPhoto.Id);
        await _repository.SaveChangesAsync();
    }
}