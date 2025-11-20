using MetalTrade.Business.Dtos;
using MetalTrade.Business.Interfaces;
using MetalTrade.DataAccess.Data;
using MetalTrade.DataAccess.Repositories;
using MetalTrade.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace MetalTrade.Business;

public class UserService : IUserService
{
    private readonly UserManagerRepository _userRepository;
    private readonly SignInManager<User> _signInManager;
    private readonly IImageUploadService _imageUploadService;

    public UserService(
        MetalTradeDbContext context, 
        UserManager<User> userManager,
        SignInManager<User> signInManager, 
        IImageUploadService imageUploadService)
    {
        _userRepository = new UserManagerRepository(context, userManager);
        _signInManager = signInManager;
        _imageUploadService = imageUploadService;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync() => await _userRepository.GetAllAsync();

    public async Task<bool> CreateUserAsync(UserDto model, string role)
    {
        string avatarPath = await _imageUploadService.UploadImageAsync(model.Photo, "avatars");

        var user = new User()
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
            return true;
        }

        return false;
    }
    // удалил RegisterUserAsync по тикету

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
        var user = login.Contains('@')
            ? await _signInManager.UserManager.FindByEmailAsync(login)
            : await _signInManager.UserManager.FindByNameAsync(login);

        if (user == null)
            return SignInResult.Failed;

        return await _signInManager.PasswordSignInAsync(
            user.UserName,
            password,
            rememberMe,
            lockoutOnFailure: false);
    }


    public async Task LogoutAsync() => await _signInManager.SignOutAsync();
    
    
}
