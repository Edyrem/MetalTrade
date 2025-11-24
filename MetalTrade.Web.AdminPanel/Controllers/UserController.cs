using MetalTrade.Business.Dtos;
using MetalTrade.Business.Interfaces;
using MetalTrade.Domain.Enums;
using MetalTrade.Web.AdminPanel.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
            var usersList = users.Select(u => new IndexUserViewModel
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                WhatsAppNumber = u.WhatsAppNumber,
                Photo = u.PhotoLink,
                Roles = u.Roles
            }).ToList();

            return View(usersList);
        }

        public async Task<IActionResult> CreateUser()
        {
            ViewData["Roles"] = new SelectListItem(UserRole.Admin.ToString(), UserRole.Admin.ToString());
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

        public async Task<IActionResult> AddRole(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            try
            {
                if(await _userService.AddToRoleAsync(user, "moderator"))
                    return RedirectToAction("Index", "User");
            }
            catch (Exception)
            {
                
            }
            ModelState.AddModelError("", "Ошибка при добавлении роли");
            return View();
        }

        public async Task<IActionResult> RemoveRole(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();
            try
            {
                if(await _userService.RemoveFromRoleAsync(user, "moderator"))
                    return RedirectToAction("Index", "User");
            }
            catch (Exception)
            {
                
            }
            ModelState.AddModelError("", "Ошибка при удалении роли");
            return View();
        }

        public async Task<IActionResult> Details(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();
            return View(user);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();
            return View(user);
        }
    }
}
