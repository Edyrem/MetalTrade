using MetalTrade.Business.Dtos;
using MetalTrade.Business.Interfaces;
using MetalTrade.Web.AdminPanel.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MetalTrade.Web.AdminPanel.Controllers
{
    [Authorize(Roles = "admin")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersWithRolesAsync();
            return View(users);
        }

        public async Task<IActionResult> CreateUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(UserViewModel model)
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
            bool success = await _userService.CreateUserAsync(dto, model.Role.ToString().ToLower());
            if (success)
                return RedirectToAction("Index", "User");

            ModelState.AddModelError("", "Ошибка при создании пользователя");
            return View(model);
        }
    }
}
