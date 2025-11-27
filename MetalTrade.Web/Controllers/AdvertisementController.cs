using AutoMapper;
using MetalTrade.Business.Dtos;
using MetalTrade.Business.Interfaces;
using MetalTrade.DataAccess.Data;
using MetalTrade.Domain.Entities;
using MetalTrade.Web.ViewModels.Advertisement;
using MetalTrade.Web.ViewModels.AdvertisementPhoto;
using MetalTrade.Web.ViewModels.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MetalTrade.Web.Controllers;

[Authorize]
public class AdvertisementController : Controller
{
    private readonly IAdvertisementService _adsService;
    private readonly IImageUploadService _imageUploadService;
    private readonly IProductService _productService;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public AdvertisementController(IAdvertisementService adsService, IUserService userService,
        IWebHostEnvironment env, IProductService productService,
        IMapper mapper, IImageUploadService imageUploadService)
    {
        _adsService = adsService;
        _userService = userService;
        _imageUploadService = imageUploadService;
        _productService = productService;
        _mapper = mapper;
    }
    public async Task<IActionResult> Create()
    {
        var productDtos = await _productService.GetAllAsync();
        CreateAdvertisementViewModel model = new()
        {
            Products = [.. productDtos.Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name
            })]
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateAdvertisementViewModel model)
    {
        var user = await _userService.GetCurrentUserAsync(HttpContext);

        if (!ModelState.IsValid || user == null)
        {
            var productDtos = await _productService.GetAllAsync();
            model.Products = [.. productDtos.Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name
            })];
            if (user == null)
                ModelState.AddModelError(string.Empty, "Пользователь не авторизован");
            return View(model);
        }

        var adsDto = _mapper.Map<AdvertisementDto>(model);
        adsDto.UserId = user.Id;

        if (model.Photoes != null && model.Photoes.Length > 0)
        {
            List<string> photoLinks = await _imageUploadService.UploadImagesAsync(model.Photoes, "advertisement");
            foreach (var link in photoLinks)
            {
                adsDto.Photoes.Add(new AdvertisementPhotoDto { PhotoLink = link });
            }
        }

        await _adsService.CreateAsync(adsDto);
        return RedirectToAction("Index");
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var adsDtos = await _adsService.GetAllAsync();
        var models = _mapper.Map<List<AdvertisementViewModel>>(adsDtos);
        return View(models);
    }
        
    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var adsDto = await _adsService.GetAsync(id);
        if (adsDto == null) return RedirectToAction("Index");

        var model = _mapper.Map<AdvertisementViewModel>(adsDto);
        return View(model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        List<string> ExistingPhotos;
        var adsDto = await _adsService.GetAsync(id);
        if (adsDto == null) return RedirectToAction("Index");
        var model = _mapper.Map<EditAdvertisementViewModel>(adsDto);
        var productDtos = await _productService.GetAllAsync();
        model.Products = [.. productDtos.Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name
            })];
        var user = await _userService.GetCurrentUserAsync(HttpContext);
        if (user == null)
            ModelState.AddModelError(string.Empty, "Пользователь не авторизован");
        if (user != null && user.Id != adsDto.UserId)
            ModelState.AddModelError(string.Empty, "Вы пытаетесь изменить чужое объявление");
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditAdvertisementViewModel model, List<IFormFile>? photoFiles)
    {
        var user = await _userService.GetCurrentUserAsync(HttpContext);
        if (!ModelState.IsValid || user == null || (user != null && user.Id != model.UserId))
        {
            var productDtos = await _productService.GetAllAsync();
            model.Products = [.. productDtos.Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name
            })];
            if (user == null)
                ModelState.AddModelError(string.Empty, "Пользователь не найден");
            if (user != null && user.Id != model.UserId)
                ModelState.AddModelError(string.Empty, "Вы пытаетесь изменить чужое объявление");
            return View(model);
        }
        var adsDto = _mapper.Map<AdvertisementDto>(model);

        adsDto.PhotoFiles = photoFiles;

        var existingAds = await _adsService.GetAsync(model.Id);
        if (existingAds != null)
        {
            adsDto.UserId = existingAds.UserId;
        }

        await _adsService.UpdateAsync(adsDto);
        return RedirectToAction("Details", new { id = model.Id });
    }

    public async Task<IActionResult> Delete(int id)
    {
        var adsDto = await _adsService.GetAsync(id);
        if (adsDto == null) 
            return RedirectToAction("Index");

        var model = _mapper.Map<DeleteAdvertisementViewModel>(adsDto);
        return View(model);
    }
        
    [HttpPost]
    public async Task<IActionResult> Delete(DeleteAdvertisementViewModel model)
    {
        await _adsService.DeleteAsync(model.Id);
        return RedirectToAction("Index");
    }
}