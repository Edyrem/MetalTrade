using MetalTrade.Business.Interfaces;
using MetalTrade.Business.Dtos;
using MetalTrade.Web.ViewModels.Profile;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MetalTrade.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using AutoMapper;

namespace MetalTrade.Web.Controllers;

[Authorize]
public class ProfileController : Controller
{
    
    private readonly IUserService _userService;
    private readonly IWebHostEnvironment _env;
    private readonly IMapper _mapper;

    public ProfileController(
        
        IUserService userService,
        IMapper mapper,
        IWebHostEnvironment env)
    {        
        _userService = userService;
        _mapper = mapper;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userService.GetCurrentUserAsync(HttpContext);
        if (user == null) return NotFound();

        var userDto = await _userService.GetUserWithAdvertisementByIdAsync(user.Id);
        
        var userViewModel = _mapper.Map<UserProfileWithAdsViewModel>(userDto);
        userViewModel.IsSupplier = await _userService.IsInRoleAsync(userDto!, "supplier");

        return View(userViewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var user = await _userService.GetCurrentUserAsync(HttpContext);        

        var userModel = _mapper.Map<UserProfileEditViewModel>(user);

        return View(userModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(UserProfileEditViewModel model)
    {
        var user = await _userService.GetCurrentUserAsync(HttpContext);
        if (user == null) return NotFound();

        if (!ModelState.IsValid)
            return View(model);

        var userDto = _mapper.Map<UserDto>(user);

        await _userService.UpdateUserAsync(userDto);

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

        var user = await _userService.GetCurrentUserAsync(HttpContext);
        if (user == null) return NotFound();

        var result = await _userService.ChangePasswordAsync(
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
