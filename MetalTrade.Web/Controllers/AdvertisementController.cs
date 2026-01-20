using AutoMapper;
using MetalTrade.Business.Dtos;
using MetalTrade.Business.Interfaces;
using MetalTrade.Domain.Enums;
using MetalTrade.Web.ViewModels.Advertisement;
using MetalTrade.Web.ViewModels.Product;
using MetalTrade.Web.ViewModels.Promotion;
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
    private readonly IMetalService _metalService;
    private readonly IAdvertisementImportService _advertisementImportService;
    private readonly ILogger<AdvertisementController> _logger;

    //private readonly ICommercialService _commercialService;
    private readonly IPromotionService _commercialService;

    public AdvertisementController(
        IAdvertisementService adsService,
        IUserService userService,
        IProductService productService,
        IMetalService metalService,
        IMapper mapper,
        ILogger<AdvertisementController> logger,
        IPromotionService commercialService,
        //ICommercialService commercialService,
        IAdvertisementImportService importService)
    {
        _adsService = adsService;
        _userService = userService;
        _productService = productService;
        _metalService = metalService;
        _mapper = mapper;
        _logger = logger;
        _commercialService = commercialService;
        _advertisementImportService = importService;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var filter = new AdvertisementFilterDto
        {
            Title = Request.Query["title"],
            City = Request.Query["city"],
            MetalTypeId = int.TryParse(Request.Query["metalTypeId"], out var mtId) ? mtId : null,
            ProductId = int.TryParse(Request.Query["productId"], out var pid) ? pid : null,
            PriceFrom = decimal.TryParse(Request.Query["priceFrom"], out var p1) ? p1 : null,
            PriceTo = decimal.TryParse(Request.Query["priceTo"], out var p2) ? p2 : null,
            DateFrom = DateTime.TryParse(Request.Query["dateFrom"], out var d1) ? d1 : null,
            DateTo = DateTime.TryParse(Request.Query["dateTo"], out var d2) ? d2 : null,
            Sort = Request.Query["sort"],
            Page = int.TryParse(Request.Query["page"], out var pg) ? pg : 1
        };

        var adsDtos = await _adsService.GetFilteredAsync(filter);
        var models = _mapper.Map<List<AdvertisementViewModel>>(adsDtos);

        var user = await _userService.GetCurrentUserAsync(HttpContext);
        bool isAdmin = user != null && await _userService.IsInRolesAsync(user, ["admin", "moderator"]);
        bool isSupplier = user != null && await _userService.IsInRoleAsync(user, "supplier");

        if (!isAdmin)
        {
            models = models.Where(a =>
                a.Status == (int)AdvertisementStatus.Active ||
                (user != null && a.UserId == user.Id)
            ).ToList();
        }
        
        ViewData["IsAdmin"] = isAdmin;
        ViewData["IsSupplier"] = isSupplier;
        ViewBag.Filter = filter;

        ViewBag.Products = await _productService.GetAllAsync();
        ViewBag.MetalTypes = await _metalService.GetAllAsync();

        return View(models.OrderByDescending(x => x.IsAd).ToList());
    }

    [HttpPost]
    public async Task<IActionResult> Load(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return RedirectToAction("Index", "Profile");
        
        var user = await _userService.GetCurrentUserAsync(HttpContext);
        if (user == null) 
            return NotFound();

        using var stream = file.OpenReadStream();
        var created = await _advertisementImportService.ImportFromExcelAsync(stream, user.Id);

        TempData["Success"] = $"Загружено объявлений без ошибок: {created}";
        return RedirectToAction("Index", "Profile");
    }


    [AllowAnonymous]
    public async Task<IActionResult> PartialList([FromQuery] AdvertisementFilterDto filter)
    {
        var adsDtos = await _adsService.GetFilteredAsync(filter);
        var models = _mapper.Map<List<AdvertisementViewModel>>(adsDtos);

        var user = await _userService.GetCurrentUserAsync(HttpContext);

        bool isAdmin = user != null && await _userService.IsInRolesAsync(user, ["admin", "moderator"]);

        if (!isAdmin)
        {
            models = models.Where(a =>
                a.Status == (int)AdvertisementStatus.Active ||
                (user != null && a.UserId == user.Id)
            ).ToList();
        }

        ViewData["IsAdmin"] = isAdmin;
        ViewData["CurrentUserId"] = user?.Id;

        return PartialView("_AdsGrid", models);
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var adsDto = await _adsService.GetAsync(id);
        if (adsDto == null)
            return RedirectToAction("Index");

        var user = await _userService.GetCurrentUserAsync(HttpContext);

        if (user == null)
            return RedirectToAction("Login", "Account",
                new { returnUrl = Url.Action("Details", new { id }) });

        var model = _mapper.Map<AdvertisementViewModel>(adsDto);

        bool isAdmin = await _userService.IsInRolesAsync(user, new[] { "admin", "moderator" });

        ViewData["IsAdmin"] = isAdmin;
        ViewData["CurrentUserId"] = user.Id;

        ViewData["AdEndDate"] = model.Commercials?.LastOrDefault()?.EndDate;
        
        return View(model);
    }

    public async Task<IActionResult> Create()
    {
        var productDtos = await _productService.GetAllAsync();
        var user = await _userService.GetCurrentUserAsync(HttpContext);
        CreateAdvertisementViewModel model = new() { Products = _mapper.Map<List<ProductViewModel>>(productDtos) };
        ViewBag.UserId = user?.Id;
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateAdvertisementViewModel model)
    {
        var user = await _userService.GetCurrentUserAsync(HttpContext);
        if(user == null)
            return Forbid();

        if (ModelState.IsValid)
        {
            var adsDto = _mapper.Map<AdvertisementDto>(model);
            adsDto.UserId = user.Id;
            await _adsService.CreateAsync(adsDto);
            return RedirectToAction("Index");
        }
        var productDtos = await _productService.GetAllAsync();
        model.Products = _mapper.Map<List<ProductViewModel>>(productDtos);
        return View(model);
    }

    [HttpGet("Edit/{id}")]
    public async Task<IActionResult> Edit(int id, string? returnUrl)
    {
        var adsDto = await _adsService.GetAsync(id);
        var user = await _userService.GetCurrentUserAsync(HttpContext);
        if (user == null)
            return Forbid();

        var isAdmin = await _userService.IsInRoleAsync(user, "admin") || await _userService.IsInRoleAsync(user, "moderator");

        if (adsDto == null)
            ModelState.AddModelError(string.Empty, "Объявление не найдено");

        else if (user.Id != adsDto.UserId && !isAdmin)
            ModelState.AddModelError(string.Empty, "Вы пытаетесь изменить чужое объявление");
        else
        {
            var model = _mapper.Map<EditAdvertisementViewModel>(adsDto);
            var productDtos = await _productService.GetAllAsync();
            model.Products = _mapper.Map<List<ProductViewModel>>(productDtos);
            return View(model);
        }
        return returnUrl != null ? Redirect(returnUrl) : RedirectToAction("Index");
        
    }

    [HttpPost("Edit/{id}")]
    public async Task<IActionResult> Edit(EditAdvertisementViewModel model)
    {
        var user = await _userService.GetCurrentUserAsync(HttpContext);
        if (user == null)
            return Forbid();
        
        var isAdmin = await _userService.IsInRoleAsync(user, "admin") || await _userService.IsInRoleAsync(user, "moderator");
        
        if (ModelState.IsValid)
        {
            if (user.Id != model.UserId && !isAdmin)
            {
                ModelState.AddModelError(string.Empty, "Вы пытаетесь изменить чужое объявление");
            }
            else
            {
                var adsDto = _mapper.Map<AdvertisementDto>(model);
                await _adsService.UpdateAsync(adsDto);
                return RedirectToAction("Details", new { id = model.Id });
            }
        }
        var productDtos = await _productService.GetAllAsync();
        model.Products = _mapper.Map<List<ProductViewModel>>(productDtos);

        return View(model);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var user = await _userService.GetCurrentUserAsync(HttpContext);
        if (user == null)
            return Forbid();

        var isAdmin = await _userService.IsInRoleAsync(user, "admin") || await _userService.IsInRoleAsync(user, "moderator");

        var adsDto = await _adsService.GetAsync(id);

        if (adsDto == null)
            ModelState.AddModelError(string.Empty, "Объявление не найдено");        
        else if (user.Id != adsDto.UserId && !isAdmin)
            ModelState.AddModelError(string.Empty, "Вы пытаетесь удалить чужое объявление");
        else
        {
            var model = _mapper.Map<DeleteAdvertisementViewModel>(adsDto);
            return View(model);
        }
        return RedirectToAction("Index");
    }
        
    [HttpPost]
    public async Task<IActionResult> Delete(DeleteAdvertisementViewModel model)
    {
        var user = await _userService.GetCurrentUserAsync(HttpContext);
        if (user == null)
            return Forbid();

        var isAdmin = await _userService.IsInRoleAsync(user, "admin") || await _userService.IsInRoleAsync(user, "moderator");

        //тут было "else if" но он ругался, и я оставил только if 
        
        if (user.Id != model.UserId && !isAdmin)
            ModelState.AddModelError(string.Empty, "Вы пытаетесь удалить чужое объявление");
        else
        {
            await _adsService.DeleteAsync(model.Id);
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAdvertisementPhotoAjax(int advertisementPhotoId, string photoLink)
    {
        try
        {
            var success = await _adsService.DeleteAdvertisementPhotoAsync( new AdvertisementPhotoDto
            {
                Id = advertisementPhotoId,
                PhotoLink = photoLink
            });
            return Json(new { success });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при удалении фото с id {PhotoId}", advertisementPhotoId);
            return Json(new { success = false });
        }
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
    public async Task<IActionResult> CreatePhoto(int id)
    {
        var adsDto = await _adsService.GetAsync(id);
        var model = _mapper.Map<AdvertisementViewModel>(adsDto);
        return View(model);
    }
    [HttpPost]
    public async Task<IActionResult> CreateAdvertisementPhotoAjax(int advertisementId, List<IFormFile> photos)
    {
        if (photos?.Any() != true)
            return Json(new { success = false });

        var newPhotos = 
            await _adsService.CreateAdvertisementPhotoAsync(new AdvertisementDto { Id = advertisementId, PhotoFiles = photos });

        if (newPhotos == null || !newPhotos.Any())
            return Json(new { success = false });

        return Json(new { success = true, photos = newPhotos });
    }
    
    [HttpPost]
    [Route("Advertisement/ActivateCommercial")]
    [Authorize(Roles = "admin,moderator")]
    public async Task<IActionResult> ActivateCommercial(PromotionActivateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var error = ModelState.Values
                .SelectMany(v => v.Errors)
                .FirstOrDefault()?.ErrorMessage;

            return BadRequest(new { message = error ?? "Некорректные данные" });
        }

        try
        {
            var user = await _userService.GetCurrentUserAsync(HttpContext);
            if (user == null)
                return Forbid();

            var viewModel = new CommercialViewModel
            {
                AdvertisementId = model.TargetId,
                //Days = model.Days,
                CreatedByUserId = user.Id,                
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(model.Days)
            };
            
            var dto = _mapper.Map<CommercialDto>(model);
            await _adsService.CreateCommercialAsync(dto);

            return Ok(new { success = true });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch
        {
            return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
        }
    }


    [HttpPost]
    [Authorize(Roles = "admin,moderator")]
    public async Task<IActionResult> DeactivateCommercial(int advertisementId)
    {
        try
        {
            await _adsService.DeactivatePromotionAsync(advertisementId);
            return Ok(new { success = true });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }







    
}