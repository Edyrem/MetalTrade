using MetalTrade.Business;
using MetalTrade.Business.Dtos;
using MetalTrade.Business.Interfaces;
using MetalTrade.DataAccess;
using MetalTrade.Domain.Entities;
using MetalTrade.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MetalTrade.Web.Controllers;

[Authorize(Roles = "admin")]
public class AdminController : Controller
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }
    
    public async Task<IActionResult> Index()
    {
        var users = await _adminService.GetAllUsersAsync();
        return View(users);
    } 
    
    public async Task<IActionResult> CreateSupplier()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateSupplier(UserViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var dto = new UserDto
        {
            UserName = model.UserName,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            WhatsAppNumber = model.WhatsAppNumber,
            Photo = model.Photo,
            Password = model.Password
        };
        bool success = await _adminService.CreateSupplierAsync(dto);

        if (success)
            return RedirectToAction("Index", "Admin");

        ModelState.AddModelError("", "Ошибка при создании пользователя");
        return View(model);
    }
}