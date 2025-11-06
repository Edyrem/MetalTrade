using MetalTrade.Business.Services;
using MetalTrade.DataAccess.Data;
using MetalTrade.Domain.Entities;
using MetalTrade.Web.ViewModels.Advertisement;
using Microsoft.AspNetCore.Mvc;

namespace MetalTrade.Web.Controllers
{
    public class AdvertisementController : Controller
    {
        AdvertisementService adsService;
        public AdvertisementController(MetalTradeDbContext context)
        {
            adsService = new AdvertisementService(context);
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
                await adsService.CreateAsync(ads);
                return Json(ads);
            }
            return View(model);
        }
    }
}
