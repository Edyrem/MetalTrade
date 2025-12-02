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

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        
        if (user == null)
            return null;

        var userDto = new UserDto {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            PhoneNumber = user.PhoneNumber,
            WhatsAppNumber = user.WhatsAppNumber,
            PhotoLink = user.Photo
        };
        return userDto;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        var userDtos = users.Select(user => new UserDto
        {
            Email = user.Email,
            UserName = user.UserName,
            PhoneNumber = user.PhoneNumber,
            WhatsAppNumber = user.WhatsAppNumber,
            PhotoLink = user.Photo
        });
        return userDtos;
    }

    public async Task<bool> AddToRoleAsync(UserDto userDto, string role)
    {
        if (userDto == null) return false;
        var user = await _userRepository.GetAsync(userDto.Id);
        if (user == null) return false;
        var result =  await _userRepository.AddToRoleAsync(user, role);
        return result.Succeeded;
    }

    public async Task<bool> RemoveFromRoleAsync(UserDto userDto, string role)
    {
        if (userDto == null) return false;
        var user = await _userRepository.GetAsync(userDto.Id);
        if (user == null) return false;
        var result = await _userRepository.RemoveFromRoleAsync(user, role);
        return result.Succeeded;
    }

    public async Task<bool> IsInRoleAsync(UserDto userDto, string role)
    {
        if (userDto == null) return false;
        var user = await _userRepository.GetAsync(userDto.Id);
        if (user == null) return false;
        return await _userRepository.IsInRoleAsync(user, role);
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync(UserDto userDto)
    {
        if (userDto == null) return Enumerable.Empty<string>();
        var user = await _userRepository.GetAsync(userDto.Id);
        if (user == null) return Enumerable.Empty<string>();
        return await _userRepository.GetUserRolesAsync(user) ?? Enumerable.Empty<string>();
    }

    public async Task<bool> CreateUserAsync(UserDto model, string role)
    {
        var avatarPath = await _imageUploadService.UploadImageAsync(model.Photo, "avatars") ?? "";

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
            if(!string.IsNullOrEmpty(role))
            {
                role = role.ToLower();
                if (role != "user")
                {
                    await _userRepository.AddToRoleAsync(user, "user");
                }
            }
            else 
            {
                role = "user";
            }
            await _userRepository.AddToRoleAsync(user, role);
            return true;
        }

        return false;
    }
    
    public async Task<bool> RegisterUserAsync(UserDto model)
    {
        string avatarPath = await _imageUploadService.UploadImageAsync(model.Photo, "avatars");

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
            await _userRepository.AddToRoleAsync(user, "User");
            return true;
        }

        return false;
    }    

    public async Task<List<UserDto>> GetAllUsersWithRolesAsync()
    {
        var users = await _userRepository.GetAllAsync();
        var result = new List<UserDto>();
        foreach (var user in users)
        {
            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                WhatsAppNumber = user.WhatsAppNumber,
                PhotoLink = user.Photo                
            };
            var role = await _userRepository.GetUserRolesAsync(user);
            userDto.Roles = role.ToList() ?? new List<string>();
            result.Add(userDto);
        }
        return result;
    }

    public async Task UpdateUserAsync(UserDto model)
    {
        var user = await _userRepository.GetAsync(model.Id);
        if (user == null) return;
        user.Email = model.Email ?? user.Email;
        user.UserName = model.UserName ?? user.UserName;
        user.PhoneNumber = model.PhoneNumber ?? user.PhoneNumber;
        user.WhatsAppNumber = model.WhatsAppNumber ?? user.WhatsAppNumber;
        if (model.Photo != null)
        {
            var avatarPath = await _imageUploadService.UploadImageAsync(model.Photo, "avatars");
            if (!string.IsNullOrEmpty(avatarPath))
            {
                user.Photo = avatarPath;
            }
        }
        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(int id)
    {
        await _userRepository.DeleteAsync(id);
        await _userRepository.SaveChangesAsync();
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
