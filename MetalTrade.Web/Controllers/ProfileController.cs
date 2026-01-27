using AutoMapper;
using MetalTrade.Business.Dtos;
using MetalTrade.Business.Interfaces;
using MetalTrade.Domain.Entities;
using MetalTrade.Web.ViewModels.Profile;
using MetalTrade.Web.ViewModels.Promotion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
        var currentUser = await _userService.GetCurrentUserAsync(HttpContext);        
        if (currentUser == null) return NotFound();

        var userDto = await _userService.GetUserWithAdvertisementByIdAsync(currentUser.Id);
            
        var userViewModel = _mapper.Map<UserProfileWithAdsViewModel>(userDto);

        var roles = await _userService.GetUserRolesAsync(userDto);
        userViewModel.Roles = roles.ToList();

        var topUserViewModel = userViewModel.TopUsers?.LastOrDefault(x => x.IsActive);
        userViewModel.IsSupplier = userViewModel.Roles.Contains("supplier");


        ViewData["TopEndDate"] = topUserViewModel?.EndDate.ToString("dd.MM.yyyy");
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

        var userDto = _mapper.Map<UserDto>(model);
            
        if (model.Photo != null)
        {
            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "avatars");
            Directory.CreateDirectory(uploadsDir);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.Photo.FileName)}";
            var fullPath = Path.Combine(uploadsDir, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await model.Photo.CopyToAsync(stream);

            userDto.PhotoLink = "/uploads/avatars/" + fileName;
        }

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
        
    [AllowAnonymous]
    public async Task<IActionResult> User(int id)
    {
        var user = await _userService.GetCurrentUserAsync(HttpContext);
        var userDto = await _userService.GetUserWithAdvertisementByIdAsync(id);
        if (userDto == null) return NotFound();

        var viewModel = _mapper.Map<UserProfileWithAdsViewModel>(userDto);
        var roles = await _userService.GetUserRolesAsync(userDto);

        viewModel.Roles = roles.ToList();
        viewModel.IsSupplier = viewModel.Roles.Contains("supplier");

        ViewBag.IsOwner = user != null && user.Id == userDto.Id;
        ViewBag.UserId = userDto.Id;

        return View("UserProfile", viewModel);
    }

        
}
