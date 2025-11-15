using MetalTrade.DataAccess.Data;
using MetalTrade.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace MetalTrade.DataAccess;

public class UserInitializer
{
    public static async Task SeedUserAsync( UserManager<User> userManager)
    {
        string password = "1qwe@QWE";
        string phoneNumber = "9999999999";
        string avatarUrl =
            "https://upload.wikimedia.org/wikipedia/commons/thumb/5/59/User-avatar.svg/2048px-User-avatar.svg.png";
        
        if (await userManager.FindByEmailAsync("supplier1@gmail.com") == null)
        {
            User supplier = new User
            {
                Email = "supplier1@gmail.com",
                UserName = "supplier1", 
                PhoneNumber = phoneNumber,
                WhatsAppNumber = phoneNumber,
                Photo = avatarUrl
            };
            IdentityResult result = await userManager.CreateAsync(supplier, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(supplier, "supplier");
            }
        }
        
        if (await userManager.FindByEmailAsync("supplier2@gmail.com") == null)
        {
            User supplier = new User
            {
                Email = "supplier2@gmail.com",
                UserName = "supplier2", 
                PhoneNumber = phoneNumber,
                WhatsAppNumber = phoneNumber,
                Photo = avatarUrl
            };
            IdentityResult result = await userManager.CreateAsync(supplier, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(supplier, "supplier");
            }
        }
        
        if (await userManager.FindByEmailAsync("moderator1@gmail.com") == null)
        {
            User moderator = new User
            {
                Email = "moderator1@gmail.com",
                UserName = "moderator1", 
                PhoneNumber = phoneNumber,
                WhatsAppNumber = phoneNumber,
                Photo = avatarUrl
            };
            IdentityResult result = await userManager.CreateAsync(moderator, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(moderator, "moderator");
            }
        }
        
        if (await userManager.FindByEmailAsync("moderator2@gmail.com") == null)
        {
            User moderator = new User
            {
                Email = "moderator2@gmail.com",
                UserName = "moderator2", 
                PhoneNumber = phoneNumber,
                WhatsAppNumber = phoneNumber,
                Photo = avatarUrl
            };
            IdentityResult result = await userManager.CreateAsync(moderator, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(moderator, "moderator");
            }
        }
    }
}