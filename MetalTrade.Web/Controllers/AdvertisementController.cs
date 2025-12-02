using AutoMapper;
using MetalTrade.Business.Dtos;
using MetalTrade.Business.Interfaces;
using MetalTrade.Web.ViewModels.Advertisement;
using MetalTrade.Web.ViewModels.AdvertisementPhoto;
using MetalTrade.Web.ViewModels.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MetalTrade.Web.Controllers;

[Authorize(Roles = "admin, moderator, supplier")]
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
        CreateAdvertisementViewModel model = new() { Products = _mapper.Map<List<ProductViewModel>>(productDtos) };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateAdvertisementViewModel model)
    {
        var user = await _userService.GetCurrentUserAsync(HttpContext);

        if (!ModelState.IsValid || user == null)
        {
            var productDtos = await _productService.GetAllAsync();
            model.Products = _mapper.Map<List<ProductViewModel>>(productDtos);
            if (user == null)
                ModelState.AddModelError(string.Empty, "Пользователь не авторизован");
            return View(model);
        }
        var adsDto = _mapper.Map<AdvertisementDto>(model);
        adsDto.UserId = user.Id;
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
        var user = await _userService.GetCurrentUserAsync(HttpContext);
        if (user != null)
            ViewBag.CurrentUserId = user.Id;
        return View(model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var adsDto = await _adsService.GetAsync(id);
        var user = await _userService.GetCurrentUserAsync(HttpContext);
        if (adsDto == null || user == null || (user != null && adsDto != null && user.Id != adsDto.UserId))
        {
            if (adsDto == null)
                ModelState.AddModelError(string.Empty, "Объявление не найдено");
            if (user == null)
                ModelState.AddModelError(string.Empty, "Пользователь не авторизован");
            if (user != null && adsDto != null && user.Id != adsDto.UserId)
                ModelState.AddModelError(string.Empty, "Вы пытаетесь изменить чужое объявление");
            return RedirectToAction("Index");
        }
        var model = _mapper.Map<EditAdvertisementViewModel>(adsDto);
        var productDtos = await _productService.GetAllAsync();
        model.Products = _mapper.Map<List<ProductViewModel>>(productDtos);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditAdvertisementViewModel model)
    {
        var user = await _userService.GetCurrentUserAsync(HttpContext);
        if (!ModelState.IsValid || user == null || (user != null && user.Id != model.UserId))
        {
            var productDtos = await _productService.GetAllAsync();
            var tempAdsDto = await _adsService.GetAsync(model.Id);
            model.Products = _mapper.Map<List<ProductViewModel>>(productDtos);
            if (tempAdsDto != null)
                model.Photoes = _mapper.Map<List<AdvertisementPhotoViewModel>>(tempAdsDto.Photoes);
            if (user == null)
                ModelState.AddModelError(string.Empty, "Пользователь не авторизован");
            if (user != null && user.Id != model.UserId)
                ModelState.AddModelError(string.Empty, "Вы пытаетесь изменить чужое объявление");
            return View(model);
        }
        var adsDto = _mapper.Map<AdvertisementDto>(model);
        await _adsService.UpdateAsync(adsDto);
        return RedirectToAction("Details", new { id = model.Id });
    }

    public async Task<IActionResult> Delete(int id)
    {
        var user = await _userService.GetCurrentUserAsync(HttpContext);
        var adsDto = await _adsService.GetAsync(id);
        bool isInvalidRequest = adsDto == null || user == null || (user != null && adsDto != null && user.Id != adsDto.UserId);
        if (isInvalidRequest)
        {
            if (adsDto == null)
                ModelState.AddModelError(string.Empty, "Объявление не найдено");
            if (user == null)
                ModelState.AddModelError(string.Empty, "Пользователь не авторизован");
            if (user != null && adsDto != null && user.Id != adsDto.UserId)
                ModelState.AddModelError(string.Empty, "Вы пытаетесь удалить чужое объявление");
            return RedirectToAction("Index");
        }
        var model = _mapper.Map<DeleteAdvertisementViewModel>(adsDto);
        return View(model);
    }
        
    [HttpPost]
    public async Task<IActionResult> Delete(DeleteAdvertisementViewModel model)
    {
        var user = await _userService.GetCurrentUserAsync(HttpContext);
        if (user == null || (user != null && user.Id != model.UserId))
        {
            if (user == null)
                ModelState.AddModelError(string.Empty, "Пользователь не авторизован");
            if (user != null && user.Id != model.UserId)
                ModelState.AddModelError(string.Empty, "Вы пытаетесь удалить чужое объявление");
            return RedirectToAction("Index");
        }
        await _adsService.DeleteAsync(model.Id);
        return RedirectToAction("Index");
    }
    [HttpPost]
    public async Task<IActionResult> DeleteAdvertisementPhoto(int advertisementPhotoId, string photoLink, int advertisementId)
    {
        await _adsService.DeleteAdvertisementPhotoAsync(new AdvertisementPhotoDto
        {
            Id = advertisementPhotoId,
            PhotoLink = photoLink
        });
        return RedirectToAction("Edit", new { Id = advertisementId});
    }
}