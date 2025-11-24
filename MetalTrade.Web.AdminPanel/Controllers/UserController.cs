using AutoMapper;
using MetalTrade.Business.Common.Mapping;
using MetalTrade.Business.Dtos;
using MetalTrade.Business.Interfaces;
using MetalTrade.Domain.Enums;
using MetalTrade.Web.AdminPanel.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MetalTrade.Web.AdminPanel.Controllers
{
    [Authorize(Roles = "admin")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IMapper mapper)
        {
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersWithRolesAsync();
            var usersList = _mapper.Map<List<IndexUserViewModel>>(users);

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

            var dto = _mapper.Map<UserDto>(model);

            bool success = false;
            if(model.Role == UserRole.Moderator)
            {
                success = await _userService.CreateUserAsync(dto, "user");
                success = false;
            }
            success = await _userService.CreateUserAsync(dto, model.Role.ToString().ToLower());
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
                await _userService.AddToRoleAsync(user, "moderator");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Ошибка при добавлении роли");
            }
            return RedirectToAction("Index", "User");
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
            var userViewModel = _mapper.Map<DeleteUserViewModel>(user);
            return View(userViewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _userService.DeleteUserAsync(id);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ошибка при удалении пользователя: " + ex.Message);
                RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
