using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MetalTrade.Business.Interfaces;
using MetalTrade.DataAccess.Data;
using MetalTrade.Domain.Entities;
using MetalTrade.Business.Dtos;
using Microsoft.AspNetCore.Http;

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

    public async Task<ProfileWithAdsDto> GetProfileAsync(User user)
    {
        bool isSupplier = await _userManager.IsInRoleAsync(user, "supplier");

        var dto = new ProfileWithAdsDto
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
            dto.Advertisements = await _context.Advertisements
                .Where(a => a.UserId == user.Id && !a.IsDeleted)
                .Include(a => a.Photoes)
                .Select(a => new AdvertisementDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Body = a.Body,
                    Price = a.Price,
                    CreateDate = a.CreateDate,
                    Photoes = a.Photoes
                        .Where(p => !p.IsDeleted)
                        .Select(p => new AdvertisementPhotoDto
                        {
                            Id = p.Id,
                            PhotoLink = p.PhotoLink,
                            AdvertisementId = p.AdvertisementId
                        })
                        .ToList()
                })
                .ToListAsync();
        }
        else
        {
            dto.Advertisements = new List<AdvertisementDto>();
        }


        return dto;
    }
        
    public async Task<bool> UpdateProfileAsync(User user, ProfileDto dto, IFormFile? photo, IWebHostEnvironment env)
    {
        user.UserName = dto.UserName;
        user.Email = dto.Email;
        user.PhoneNumber = dto.PhoneNumber;
        user.WhatsAppNumber = dto.WhatsAppNumber;
        
        user.NormalizedUserName = _userManager.NormalizeName(user.UserName);
        user.NormalizedEmail = _userManager.NormalizeEmail(user.Email);
        
        if (photo != null && photo.Length > 0)
        {
            string uploadsFolder = Path.Combine(env.WebRootPath, "images", "avatars");
            Directory.CreateDirectory(uploadsFolder);

            string fileName = Guid.NewGuid() + Path.GetExtension(photo.FileName);
            string path = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(path, FileMode.Create);
            await photo.CopyToAsync(stream);

            user.Photo = "/images/avatars/" + fileName;
        }

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public Task<ProfileDto> GetProfileEditModelAsync(User user)
    {
        return Task.FromResult(new ProfileDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            WhatsAppNumber = user.WhatsAppNumber,
            PhotoPath = user.Photo
        });
    }
}
