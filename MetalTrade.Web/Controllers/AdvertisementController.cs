using MetalTrade.Business.Services;
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
        private readonly AdvertisementService adsService;
        private readonly AdvertisementPhotoService adsPhotoService;
        private readonly AdvertisementPhotoSaveService adsPhotoSaveService;
        private readonly UserManager<User> _userManager;

        public AdvertisementController(MetalTradeDbContext context, IWebHostEnvironment env, UserManager<User> userManager)
        {
            adsService = new AdvertisementService(context);
            adsPhotoService = new AdvertisementPhotoService(context);
            adsPhotoSaveService = new AdvertisementPhotoSaveService(env);
            _userManager = userManager;
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                Advertisement ads = new()
                {
                    Title = model.Title ?? string.Empty,
                    Body = model.Body ?? string.Empty,
                    Price = model.Price,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber ?? string.Empty,
                    City = model.City
                };
                User? user = await _userManager.GetUserAsync(User);
                if (user != null)
                    ads.UserId = user.Id;
                int adsId = await adsService.CreateAsync(ads);
                if (model.Images != null)
                {
                    List<string> photoLinks = await adsPhotoSaveService.SavePhotosAsync(model.Images);
                    if (photoLinks.Count > 0)
                    {
                        foreach (var link in photoLinks)
                        {
                            var adsPhoto = new AdvertisementPhoto()
                            {
                                PhotoLink = link,
                                AdvertisementId = adsId
                            };
                            await adsPhotoService.CreateAsync(adsPhoto);
                        }
                    }
                }
                return Json(ads);
            }
            return View(model);
        }
    }
}
