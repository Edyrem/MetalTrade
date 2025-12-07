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
    
    private readonly IUserService _userService;    
    private readonly UserManager<User> _userManager;
    private readonly IWebHostEnvironment _env;

    public ProfileController(
        
        IUserService userService,
        UserManager<User> userManager, 
        IWebHostEnvironment env)
    {
        
        _userService = userService;
        _userManager = userManager;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var dto = await _userService.GetUserWithAdvertisementByIdAsync(user.Id);        

        var vm = new UserProfileWithAdsViewModel
        {
            UserName = dto.UserName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            WhatsAppNumber = dto.WhatsAppNumber,
            PhotoPath = dto.PhotoLink,
            IsSupplier = await _userService.IsInRoleAsync(dto, "supplier"),
            Advertisements = dto.Advertisements,
        };

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var user = await _userService.GetCurrentUserAsync(HttpContext);        

        var vm = new UserProfileEditViewModel
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            WhatsAppNumber = user.WhatsAppNumber,
            PhotoPath = user.PhotoLink,
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

        var dto = new UserDto
        {
            Id = model.Id,
            UserName = model.UserName,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            WhatsAppNumber = model.WhatsAppNumber,
            PhotoLink = model.PhotoPath
        };

        await _userService.UpdateUserAsync(dto);

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
