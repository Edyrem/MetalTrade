using AutoMapper;
using MetalTrade.Business.Dtos;
using MetalTrade.Business.Interfaces;
using MetalTrade.Domain.Enums;
using MetalTrade.Web.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MetalTrade.Web.AdminPanel.Controllers
{
    [Authorize(Roles = "admin,moderator")]
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
            var usersList = _mapper.Map<List<UserViewModel>>(users);

            return View(usersList);
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id == 0)
                return NotFound();
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
                return NotFound();

            var userViewModel = _mapper.Map<UserViewModel>(user);
            userViewModel.Roles = (List<string>)await _userService.GetUserRolesAsync(user);
            return View(userViewModel);
        }

        public async Task<IActionResult> CreateUser()
        {
            ViewData["Roles"] = new SelectListItem(UserRole.Admin.ToString(), UserRole.Admin.ToString());
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = _mapper.Map<UserDto>(model);

            var success = await _userService.CreateUserAsync(dto, model.Role.ToString().ToLower());

            if (success)
            {
                return RedirectToAction("Index", "User");
            }

            ModelState.AddModelError("", "Ошибка при создании пользователя");
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();
            var userViewModel = _mapper.Map<EditUserViewModel>(user);
            return View(userViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditUserViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
                return View(model);

            var dto = _mapper.Map<UserDto>(model);
            try
            {
                await _userService.UpdateUserAsync(dto);
                return RedirectToAction("Index", "User");
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError("", "Ошибка при обновлении пользователя");
                return View(model);
            }
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddRole(int id, string role, string? page)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            try
            {
                await _userService.AddToRoleAsync(user, role);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Ошибка при добавлении роли");
            }
            if (page == null)
                return RedirectToAction("Index", "User");
            else
                return RedirectToAction(page, "User", new { id });
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> RemoveRole(int id, string role, string? page)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();
            try
            {
                await _userService.RemoveFromRoleAsync(user, role);                

            }
            catch (Exception)
            {
                
            }
            if (page == null)
                return RedirectToAction("Index", "User");
            else
                return RedirectToAction(page, "User", new { id });
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
