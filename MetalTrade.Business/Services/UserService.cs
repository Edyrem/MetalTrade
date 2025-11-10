using MetalTrade.Business.Dtos;
using MetalTrade.Business.Interfaces;
using MetalTrade.DataAccess.Data;
using MetalTrade.DataAccess.Repositories;
using MetalTrade.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;

namespace MetalTrade.Business.Services;

public class UserService : IUserService
{
    private readonly UserManagerRepository _userRepository;
    private readonly IWebHostEnvironment _env;
    private readonly SignInManager<User> _signInManager;

    public UserService(MetalTradeDbContext context, UserManager<User> userManager, SignInManager<User> signInManager, IWebHostEnvironment env)
    {
        _userRepository = new UserManagerRepository(context, userManager);
        _signInManager = signInManager;
        _env = env;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync() => await _userRepository.GetAllAsync();

    public async Task<bool> CreateUserAsync(UserDto model, string role, bool autoLogin = false)
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

        var user = new User
        {
            Email = model.Email,
            UserName = model.UserName,
            PhoneNumber = model.PhoneNumber,
            WhatsAppNumber = model.WhatsAppNumber,
            Photo = avatarPath
        };

        var result = await _userRepository.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            await _userRepository.AddToRoleAsync(user, role);
            
            var userManager = _signInManager.UserManager;
            var freshUser = await userManager.FindByNameAsync(user.UserName);

            if (freshUser != null)
            {
                await _signInManager.SignInAsync(freshUser, isPersistent: false);
            }

            return true;
        }


        await _userRepository.AddToRoleAsync(user, role);
        
        if (autoLogin)
        {
            var createdUser = await _userRepository.GetByEmailAsync(model.Email);
            if (createdUser != null)
            {
                var signInResult = await _signInManager.PasswordSignInAsync(
                    createdUser.UserName,
                    model.Password,
                    isPersistent: false,
                    lockoutOnFailure: false
                );

                if (!signInResult.Succeeded)
                    Console.WriteLine("⚠ Не удалось автоматически войти после регистрации");
            }
        }

        return true;
    }


    public async Task<Dictionary<User, string?>> GetAllUsersWithRolesAsync()
    {
        var users = await _userRepository.GetAllAsync();
        var result = new Dictionary<User, string?>();
        foreach (var user in users)
        {
            var role = await _userRepository.GetUserRoleAsync(user);
            result[user] = role;
        }

        return result;
    }
    
    public async Task<SignInResult> LoginAsync(string login, string password, bool rememberMe)
    {
        return await _signInManager.PasswordSignInAsync(login, password, rememberMe, false);
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }
    
}