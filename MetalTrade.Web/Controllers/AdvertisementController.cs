using AutoMapper;
using MetalTrade.Business.Dtos;
using MetalTrade.Business.Interfaces;
using MetalTrade.Domain.Enums;
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
    private readonly IProductService _productService;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public AdvertisementController(IAdvertisementService adsService, IUserService userService,
        IWebHostEnvironment env, IProductService productService,
        IMapper mapper)
    {
        _adsService = adsService;
        _userService = userService;
        _productService = productService;
        _mapper = mapper;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var adsDtos = await _adsService.GetAllAsync();
        var models = _mapper.Map<List<AdvertisementViewModel>>(adsDtos);

        var user = await _userService.GetCurrentUserAsync(HttpContext);
        bool isAdmin = true;
        if(!(await _userService.IsInRoleAsync(user, "admin") || await _userService.IsInRoleAsync(user, "moderator")))
        {
            models = models.Where(a => a.Status == (int)AdvertisementStatus.Active).ToList();
            isAdmin = false;
        }
        ViewData["IsAdmin"] = isAdmin;
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

    public async Task<IActionResult> Create()
    {
        var productDtos = await _productService.GetAllAsync();
        CreateAdvertisementViewModel model = new() { Products = _mapper.Map<List<ProductViewModel>>(productDtos) };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateAdvertisementViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userService.GetCurrentUserAsync(HttpContext);
            if (user != null)
            {
                var adsDto = _mapper.Map<AdvertisementDto>(model);
                adsDto.UserId = user.Id;
                await _adsService.CreateAsync(adsDto);
                return RedirectToAction("Index");                
            }
            ModelState.AddModelError(string.Empty, "Пользователь не авторизован");
        }
        var productDtos = await _productService.GetAllAsync();
        model.Products = _mapper.Map<List<ProductViewModel>>(productDtos);
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

    [Authorize(Roles = "admin,moderator")]
    public async Task<IActionResult> ApproveAdvertisement(int id)
    {
        try
        {
            await _adsService.ApproveAsync(id);
        } 
        catch (Exception e)
        {
            ModelState.AddModelError(string.Empty, e.Message);
        }
        return RedirectToAction("Index");
    }

    [Authorize(Roles = "admin,moderator")]
    public async Task<IActionResult> RejectAdvertisement(int id)
    {
        try
        {
            await _adsService.RejectAsync(id);
        }
        catch (Exception e)
        {
            ModelState.AddModelError(string.Empty, e.Message);
        }
        return RedirectToAction("Index");
    }

    [Authorize(Roles = "admin,moderator")]
    public async Task<IActionResult> ArchiveAdvertisement(int id)
    {
        try
        {
            await _adsService.ArchiveAsync(id);
        }
        catch (Exception e)
        {
            ModelState.AddModelError(string.Empty, e.Message);
        }
        return RedirectToAction("Index");
    }
}