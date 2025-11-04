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
    private readonly MetalTradeDbContext _context;
    private readonly UserManager<User> _userManager;

    public AdminController(MetalTradeDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    public async Task<IActionResult> Index()
    {
        List<User> users = _context.Users.Skip(1).ToList();
        return View(users);
    } 
    
    public async Task<IActionResult> CreateSupplier()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateSupplier(UserViewModel model, [FromServices] IWebHostEnvironment env)
    {
        if(ModelState.IsValid)
        {
            string avatarPath = "";
            if (model.Photo != null && model.Photo.Length > 0)
            {
                string uploadsFolder = Path.Combine(env.WebRootPath, "images", "avatars");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);
                
                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Photo.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Photo.CopyToAsync(stream);
                }
                avatarPath = "/images/avatars/" + uniqueFileName;
            }
            
            var user = new User()
            {
                Email = model.Email,
                UserName = model.UserName,
                PhoneNumber = model.PhoneNumber,
                WhatsAppNumber = model.WhatsAppNumber,
                Photo = avatarPath
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "supplier");
                return RedirectToAction("Index", "Admin");
            }
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
        }
        return View(model);
    }
}