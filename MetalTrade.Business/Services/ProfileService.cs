
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MetalTrade.Business.Interfaces;
using MetalTrade.DataAccess.Data;
using MetalTrade.Domain.Entities;
using MetalTrade.Application.ViewModels;


namespace MetalTrade.Business;

public class ProfileService : IProfileService
{
    private readonly MetalTradeDbContext _context;
    private readonly UserManager<User> _userManager;

    public ProfileService(MetalTradeDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<UserProfileWithAdsViewModel> GetProfileAsync(User user)
    {
        var roles = (await _userManager.GetRolesAsync(user)).ToList();
        bool isSupplier = roles.Any(r => string.Equals(r, "supplier", StringComparison.OrdinalIgnoreCase));

        if (!isSupplier)
        {
            var anyAd = await _context.Advertisements.AnyAsync(a => a.UserId == user.Id && !a.IsDeleted);
            if (anyAd) isSupplier = true;
        }

        var model = new UserProfileWithAdsViewModel
        {
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            WhatsAppNumber = user.WhatsAppNumber,
            PhotoPath = user.Photo,
            IsSupplier = isSupplier
        };

        if (isSupplier)
        {
            model.Advertisements = await _context.Advertisements
                .Where(a => a.UserId == user.Id && !a.IsDeleted)
                .Include(a => a.Photoes)
                .ToListAsync();
        }
        else
        {
            model.Favorites = await _context.Favorites
                .Where(f => f.UserId == user.Id && !f.IsDeleted)
                .Include(f => f.Advertisement).ThenInclude(a => a.Photoes)
                .Select(f => f.Advertisement)
                .ToListAsync();
        }

        return model;
    }

    public async Task<bool> UpdateProfileAsync(User user, UserProfileViewModel model, IWebHostEnvironment env)
    {
        user.UserName = model.UserName;
        user.Email = model.Email;
        user.PhoneNumber = model.PhoneNumber;
        user.WhatsAppNumber = model.WhatsAppNumber;

        user.NormalizedUserName = _userManager.NormalizeName(user.UserName);
        user.NormalizedEmail = _userManager.NormalizeEmail(user.Email);

        if (model.Photo != null && model.Photo.Length > 0)
        {
            string uploadsFolder = Path.Combine(env.WebRootPath, "images", "avatars");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string fileName = Guid.NewGuid() + Path.GetExtension(model.Photo.FileName);
            string path = Path.Combine(uploadsFolder, fileName);
            using var stream = new FileStream(path, FileMode.Create);
            await model.Photo.CopyToAsync(stream);

            user.Photo = "/images/avatars/" + fileName;
        }

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }
    public async Task<UserProfileViewModel> GetProfileEditModelAsync(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));
        
        return new UserProfileViewModel
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            WhatsAppNumber = user.WhatsAppNumber,
            PhotoPath = user.Photo
        };
    }

}
