using MetalTrade.Business.Dtos;
using MetalTrade.Business.Interfaces;
using MetalTrade.DataAccess;
using MetalTrade.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;

namespace MetalTrade.Business;

public class AdminService : IAdminService
{
    private readonly MetalTradeDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly IWebHostEnvironment _env;

    public AdminService(MetalTradeDbContext context, UserManager<User> userManager, IWebHostEnvironment env)
    {
        _context = context;
        _userManager = userManager;
        _env = env;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return _context.Users.Skip(1).ToList();
    }

    public async Task<bool> CreateSupplierAsync(UserDto model)
    {
        string avatarPath = "";
        if (model.Photo != null && model.Photo.Length > 0)
        {
            string uploadsFolder = Path.Combine(_env.WebRootPath, "images", "avatars");
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
            return true;
        }

        return false;
    }
}