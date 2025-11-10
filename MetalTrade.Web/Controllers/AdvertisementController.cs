using MetalTrade.Business.Dtos;
using MetalTrade.Business.Interfaces;
using MetalTrade.DataAccess.Data;
using MetalTrade.Domain.Entities;
using MetalTrade.Web.Services.Advertisement;
using MetalTrade.Web.ViewModels.Advertisement;
using MetalTrade.Web.ViewModels.AdvertisementPhoto;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
            return View(new CreateViewModel { Products = [] });
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
            List<AdvertisementViewModel> models = [];
            foreach (var dto in adsDtos)
            {
                AdvertisementViewModel model = new()
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
                    ProductId = dto.ProductId,
                    Product = new() { Id = dto.ProductId, Name = dto.Product.Name }
                };
                foreach (var photo in dto.Photoes)
                {
                    model.Photoes.Add(new AdvertisementPhotoViewModel
                    {
                        Id = photo.Id,
                        PhotoLink = photo.PhotoLink,
                        AdvertisementId = photo.AdvertisementId
                    });
                }
                models.Add(model);
            }
            return View(models);
        }
        public async Task<IActionResult> Details(int id)
        {
            AdvertisementDto? adsDto = await _adsService.GetAsync(id);
            if (adsDto != null)
            {
                AdvertisementViewModel model = new()
                {
                    Id = adsDto.Id,
                    Title = adsDto.Title,
                    Body = adsDto.Body,
                    Price = adsDto.Price,
                    CreateDate = adsDto.CreateDate,
                    Address = adsDto.Address,
                    PhoneNumber = adsDto.PhoneNumber,
                    City = adsDto.City,
                    Status = adsDto.Status,
                    IsTop = adsDto.IsTop,
                    IsAd = adsDto.IsAd,
                    ProductId = adsDto.ProductId,
                    Product = new() { Id = adsDto.ProductId, Name = adsDto.Product.Name }
                };
                foreach (var photo in adsDto.Photoes)
                {
                    model.Photoes.Add( new AdvertisementPhotoViewModel
                    {
                        Id = photo.Id,
                        PhotoLink = photo.PhotoLink,
                        AdvertisementId = photo.AdvertisementId
                    });
                }
                return View(model);
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Edit(int id)
        {
            AdvertisementDto? adsDto = await _adsService.GetAsync(id);
            if (adsDto != null)
            {
                EditViewModel model = new()
                {
                    Id = adsDto.Id,
                    Title = adsDto.Title,
                    Body = adsDto.Body,
                    Price = adsDto.Price,
                    Address = adsDto.Address,
                    PhoneNumber = adsDto.PhoneNumber,
                    City = adsDto.City,
                    ProductId = adsDto.ProductId,
                    Products = []
                };
                return View(model);
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Edit(EditViewModel model)
        {
            AdvertisementDto adsDto = new()
            {
                Id = model.Id,
                Title = model.Title ?? string.Empty,
                Body = model.Body ?? string.Empty,
                Price = model.Price,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber ?? string.Empty,
                City = model.City,
                ProductId = model.ProductId,
            };
            if (model.Photoes != null)
            {
                List<string> photoLinks = await _photoSaveService.SavePhotosAsync(model.Photoes);
                foreach (var link in photoLinks)
                {
                    adsDto.Photoes.Add(new AdvertisementPhotoDto { PhotoLink = link });
                }
            }
            await _adsService.UpdateAsync(adsDto);
            return RedirectToAction("Details", new { id = model.Id});
        }
        public async Task<IActionResult> Delete(int id)
        {
            AdvertisementDto? adsDto = await _adsService.GetAsync(id);
            if (adsDto != null)
                return View(new DeleteViewModel { Id = adsDto.Id, Title = adsDto.Title });
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(DeleteViewModel model)
        {
            await _adsService.DeleteAsync(model.Id);
            return RedirectToAction("Index");
        }
    }
}
