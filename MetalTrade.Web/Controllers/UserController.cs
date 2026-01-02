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
        
        public async Task<IActionResult> Index(UserFilterViewModel filterVm)
        {
            filterVm.Page = filterVm.Page <= 0 ? 1 : filterVm.Page;
            var filter = _mapper.Map<UserFilterDto>(filterVm);
            
            var currentUser = await _userService.GetCurrentUserAsync(HttpContext);
            var users = await _userService.GetFilteredAsync(filter, currentUser);
            var usersList = _mapper.Map<List<UserViewModel>>(users);
            
            return View(usersList);
        }
        
        public async Task<IActionResult> Filter(UserFilterViewModel filterVm)
        {
            var filter = _mapper.Map<UserFilterDto>(filterVm);
            
            var currentUser = await _userService.GetCurrentUserAsync(HttpContext);
            var users = await _userService.GetFilteredAsync(filter, currentUser);
            var usersVm = _mapper.Map<List<UserViewModel>>(users);
            
            var totalUsers = await _userService.GetFilteredCountAsync(filter, currentUser);
            ViewBag.Page = filter.Page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalUsers / (double)filter.PageSize);
            
            return PartialView("_UsersPartialView", usersVm);
        }
        
        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRoleAjax([FromBody] ChangeRoleViewModel model)
        {
            var dto = _mapper.Map<ChangeRoleDto>(model);
            var user = await _userService.GetUserByIdAsync(dto.UserId);
            if (user == null) 
                return NotFound();

            if (model.isAdd)
                await _userService.AddToRoleAsync(user, dto.Role);
            else
                await _userService.RemoveFromRoleAsync(user, dto.Role);

            return Ok();
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id == 0)
                return NotFound();
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
                return NotFound();

            var currentUser = await _userService.GetCurrentUserAsync(HttpContext);
            if (await _userService.IsInRoleAsync(currentUser!, "moderator") && await _userService.IsInRoleAsync(user, "moderator"))
            {
                ModelState.AddModelError("", "У вас нет прав для просмотра этой страницы");
                return RedirectToAction("Index");
            }

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
            var currentUser = await _userService.GetCurrentUserAsync(HttpContext);
            if(await _userService.IsInRoleAsync(currentUser!, "moderator") && await _userService.IsInRoleAsync(user, "moderator"))
            {
                return Forbid();
            }            
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
