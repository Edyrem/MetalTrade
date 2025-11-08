using MetalTrade.Business.Dtos;
using MetalTrade.Business.Services;
using MetalTrade.DataAccess.Data;
using MetalTrade.Domain.Entities;
using MetalTrade.Web.ViewModels.Advertisement;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MetalTrade.Web.Controllers
{
    public class AdvertisementController : Controller
    {
        private readonly AdvertisementService _adsService;
        private readonly UserManager<User> _userManager;

        public AdvertisementController(AdvertisementService adsService, UserManager<User> userManager)
        {
            _adsService = adsService;
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
                AdvertisementDto adsDto = new()
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
                    adsDto.UserId = user.Id;
                return Json(adsDto);
            }
            return View(model);
        }
    }
}
