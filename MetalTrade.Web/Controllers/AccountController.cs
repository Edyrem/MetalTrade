using AutoMapper;
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
        private readonly IMapper _mapper;

        public AccountController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
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

            var dto =  _mapper.Map<UserDto>(model);

            var success = await _userService.CreateUserAsync(dto, "user");
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
        
        [HttpPost]
        public async Task<IActionResult> UploadPhoto(IFormFile photo)
        {
            if (photo == null || photo.Length == 0)
                return BadRequest();

            var fileName = Guid.NewGuid() + Path.GetExtension(photo.FileName);
            var path = Path.Combine("wwwroot/uploads", fileName);

            Directory.CreateDirectory("wwwroot/uploads");

            using (var stream = new FileStream(path, FileMode.Create))
                await photo.CopyToAsync(stream);

            return Json(new { url = "/uploads/" + fileName });
        }

    }
}