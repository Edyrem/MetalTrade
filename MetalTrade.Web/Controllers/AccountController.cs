using MetalTrade.Business.Interfaces;
using MetalTrade.Web.ViewModel;
using MetalTrade.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MetalTrade.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _account;

        public AccountController(IAccountService account)
        {
            _account = account;
        }

        [HttpGet]
        public IActionResult Register() => View();
        
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _account.RegisterAsync(
                model.UserName,
                model.Email,
                model.Password,
                model.PhoneNumber,
                model.WhatsAppNumber,
                model.Photo);

            if (result.Succeeded)
                return RedirectToAction("Index", "Home");

            foreach (var err in result.Errors)
                ModelState.AddModelError("", err.Description);

            return View(model);
        }


        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _account.LoginAsync(
                model.Login, model.Password, model.RememberMe);

            if (result.Succeeded)
                return RedirectToAction("Index", "Home");

            ModelState.AddModelError("", "Неверный логин или пароль");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _account.LogoutAsync();
            return RedirectToAction("Login");
        }
    }
}