using MetalTrade.Business.Dtos;
using MetalTrade.Business.Interfaces;
using MetalTrade.Web.ViewModel;
using MetalTrade.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
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
                    return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Ошибка при регистрации пользователя.");
            return View(model);
        }


        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _userService.LoginAsync(model.Login, model.Password, model.RememberMe);

            if (result.Succeeded)
                return RedirectToAction("Index", "Home");

            ModelState.AddModelError("", "Неверный логин или пароль.");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _userService.LogoutAsync();
            return RedirectToAction("Login");
        }
    }
}