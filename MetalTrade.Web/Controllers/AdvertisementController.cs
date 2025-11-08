using MetalTrade.Business.Dtos;
using MetalTrade.Business.Interfaces;
using MetalTrade.DataAccess.Data;
using MetalTrade.Domain.Entities;
using MetalTrade.Web.Services.Advertisement;
using MetalTrade.Web.ViewModels.Advertisement;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MetalTrade.Web.Controllers
{
    public class AdvertisementController : Controller
    {
        private readonly IAdvertisementService _adsService;
        private readonly UserManager<User> _userManager;
        private readonly AdvertisementPhotoSaveService _photoSaveService;

        public AdvertisementController(IAdvertisementService adsService, UserManager<User> userManager,
            IWebHostEnvironment env)
        {
            _adsService = adsService;
            _userManager = userManager;
            _photoSaveService = new AdvertisementPhotoSaveService(env);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            User? user = await _userManager.GetUserAsync(User);
            if (ModelState.IsValid && user != null)
            {
                AdvertisementDto adsDto = new()
                {
                    Title = model.Title ?? string.Empty,
                    Body = model.Body ?? string.Empty,
                    Price = model.Price,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber ?? string.Empty,
                    City = model.City,
                    ProductId = model.ProductId,
                    UserId = user.Id
                };
                if (model.Photoes != null)
                {
                    List<string> photoLinks = await _photoSaveService.SavePhotosAsync(model.Photoes);
                    foreach (var link in photoLinks)
                    {
                        adsDto.Photoes.Add( new AdvertisementPhotoDto{ PhotoLink = link});
                    }
                }
                await _adsService.CreateAsync(adsDto);
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }
        public async Task<IActionResult> Index()
        {
            List<AdvertisementDto> adsDtos = await _adsService.GetAllAsync();
            List<IndexViewModel> models = [];
            foreach (var dto in adsDtos)
            {
                models.Add(new IndexViewModel
                {
                    Id = dto.Id,
                    Title = dto.Title,
                    Body = dto.Body,
                    Price = dto.Price,
                    CreateDate = dto.CreateDate,
                    Address = dto.Address,
                    PhoneNumber = dto.PhoneNumber,
                    City = dto.City,
                    Status = dto.Status,
                    IsTop = dto.IsTop,
                    IsAd = dto.IsAd,
                    Photoes = dto.Photoes
                });
            }
            return View(models);
        }
    }
}
