using MetalTrade.Business.Interfaces;
using MetalTrade.Business.Dtos;
using MetalTrade.Web.ViewModels.Profile;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MetalTrade.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace MetalTrade.Web.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly IProfileService _profileService;
    private readonly UserManager<User> _userManager;
    private readonly IWebHostEnvironment _env;

    public ProfileController(
        IProfileService profileService, 
        UserManager<User> userManager, 
        IWebHostEnvironment env)
    {
        _profileService = profileService;
        _userManager = userManager;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var dto = await _profileService.GetProfileAsync(user);

        var vm = new UserProfileWithAdsViewModel
        {
            UserName = dto.UserName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            WhatsAppNumber = dto.WhatsAppNumber,
            PhotoPath = dto.PhotoPath,
            IsSupplier = dto.IsSupplier,
            Advertisements = dto.Advertisements,
            
        };

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var dto = await _profileService.GetProfileEditModelAsync(user);

        var vm = new UserProfileEditViewModel
        {
            Id = dto.Id,
            UserName = dto.UserName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            WhatsAppNumber = dto.WhatsAppNumber,
            PhotoPath = dto.PhotoPath
        };

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(UserProfileEditViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        if (!ModelState.IsValid)
            return View(model);

        var dto = new ProfileDto
        {
            Id = model.Id,
            UserName = model.UserName,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            WhatsAppNumber = model.WhatsAppNumber,
            PhotoPath = model.PhotoPath
        };
        
        bool ok = await _profileService.UpdateProfileAsync(user, dto, model.Photo, _env);

        if (!ok)
        {
            var result = await _userManager.FindByIdAsync(user.Id.ToString()) != null
                ? await _userManager.UpdateAsync(user)
                : null;

            if (result != null && !result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return View(model);
            }
            
            ModelState.AddModelError("", "Ошибка при сохранении профиля");
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }


    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View(new ChangePasswordViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var result = await _userManager.ChangePasswordAsync(
            user,
            model.OldPassword,
            model.NewPassword
        );

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        return RedirectToAction("Index");
    }
}
