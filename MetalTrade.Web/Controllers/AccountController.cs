using MetalTrade.Business.Dtos;
using MetalTrade.Business.Interfaces;
using MetalTrade.Domain.Entities;
using MetalTrade.Web.ViewModel;
using MetalTrade.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MetalTrade.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Advertisement");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Advertisement");
            if (!ModelState.IsValid)
                return View(model);

            var dto = new UserDto
            {
                Email = model.Email,
                UserName = model.UserName,
                PhoneNumber = model.PhoneNumber,
                WhatsAppNumber = model.WhatsAppNumber,
                Password = model.Password,
                Photo = model.Photo
            };

            var success = await _userService.RegisterUserAsync(dto);
            if (success)
            {
                var loginResult = await _userService.LoginAsync(model.UserName, model.Password, false);
                if (loginResult.Succeeded)
                    return RedirectToAction("Index", "Advertisement");
            }

            ModelState.AddModelError("", "Ошибка при регистрации пользователя.");
            return View(model);
        }


        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Advertisement");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Advertisement");
            if (!ModelState.IsValid)
                return View(model);

            var result = await _userService.LoginAsync(model.Login, model.Password, model.RememberMe);

            if (result.Succeeded)
                return RedirectToAction("Index", "Advertisement");

            ModelState.AddModelError("", "Неверный логин или пароль.");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            if (User.Identity != null && !User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Advertisement");
            await _userService.LogoutAsync();
            return RedirectToAction("Login");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}