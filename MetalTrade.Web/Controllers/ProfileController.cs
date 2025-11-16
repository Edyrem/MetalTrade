using MetalTrade.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MetalTrade.Business.Interfaces;
using MetalTrade.Domain.Entities;
using Microsoft.AspNetCore.Identity;


namespace MetalTrade.Web.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IProfileService _profileService;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _env;

        public ProfileController(IProfileService profileService, UserManager<User> userManager, IWebHostEnvironment env)
        {
            _profileService = profileService;
            _userManager = userManager;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var model = await _profileService.GetProfileAsync(user);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var model = await _profileService.GetProfileEditModelAsync(user);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            bool updated = await _profileService.UpdateProfileAsync(user, model, _env);
            if (!updated) ModelState.AddModelError("", "Ошибка при сохранении профиля");

            return RedirectToAction(nameof(Index));
        }
    }

}
