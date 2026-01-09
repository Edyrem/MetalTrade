using AutoMapper;
using MetalTrade.Business.Dtos;
using MetalTrade.Business.Interfaces;
using MetalTrade.DataAccess.Data;
using MetalTrade.DataAccess.Repositories;
using MetalTrade.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MetalTrade.Business;

public class UserService : IUserService
{
    private readonly UserManagerRepository _userRepository;
    private readonly IImageUploadService _imageUploadService;
    private readonly SignInManager<User> _signInManager;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;

    public UserService(
        MetalTradeDbContext context, 
        UserManager<User> userManager,
        SignInManager<User> signInManager, 
        IImageUploadService imageUploadService,
        IMapper mapper)
    {
        _userRepository = new UserManagerRepository(context, userManager);
        _imageUploadService = imageUploadService;
        _mapper = mapper;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        
        if (user == null)
            return null;

        var userDto = _mapper.Map<UserDto>(user);
        return userDto;
    }

    public async Task<UserDto?> GetUserWithAdvertisementByIdAsync(int id)
    {
        var user = await _userRepository.GetWithAdvertisementsAsync(id);
        
        if (user == null)
            return null;

        var userDto = _mapper.Map<UserDto>(user);

        return userDto;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);
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

    public async Task<bool> IsInRolesAsync(UserDto userDto, string[] roles)
    {
        if (userDto == null) return false;
        var user = await _userRepository.GetAsync(userDto.Id);
        if (user == null) return false;
        var userRoles = await _userRepository.GetUserRolesAsync(user);

        return userRoles != null ? userRoles.Any(r => roles.Contains(r)) : false;
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
        model.PhotoLink = await _imageUploadService.UploadImageAsync(model.Photo, "avatars") ?? "";

        var user = _mapper.Map<User>(model);

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

    public async Task<List<UserDto>> GetAllUsersWithRolesAsync()
    {
        var users = await _userRepository.GetAllAsync();        
        var result = _mapper.Map<List<UserDto>>(users);
        foreach(var userDto in result)
        {
            var user = _mapper.Map<User>(userDto);
            var roles = await _userRepository.GetUserRolesAsync(user!);
            userDto.Roles = roles?.ToList() ?? new List<string>();
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
            ? await _userRepository.GetByEmailAsync(login)
            : await _userRepository.GetByUserNameAsync(login);

        if (user == null)
            return SignInResult.Failed;

        return await _signInManager.PasswordSignInAsync(
            user.UserName,
            password,
            rememberMe,
            lockoutOnFailure: false);
    }

    public async Task LogoutAsync() => await _signInManager.SignOutAsync();

    public async Task<IdentityResult> ChangePasswordAsync(UserDto userDto, string currentPassword, string newPassword)
    {
        if (userDto == null) 
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });

        var user = await _userRepository.GetAsync(userDto.Id);

        if (user == null) 
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });

        return await _userRepository.ChangePasswordAsync(user, currentPassword, newPassword);
    }

    public async Task<UserDto?> GetCurrentUserAsync(HttpContext context) =>
        _mapper.Map<UserDto?>(await _userRepository.GetCurrentUserAsync(context));
    
    
    public async Task<List<UserDto>> GetFilteredAsync(UserFilterDto filter, UserDto? currentUser)
    {
        var query = _userRepository.CreateFilter().Where(u => u.Id != 1);

        if (!string.IsNullOrWhiteSpace(filter.UserName))
            query = _userRepository.FilterUserName(query, filter.UserName);

        if (!string.IsNullOrWhiteSpace(filter.Email))
            query = _userRepository.FilterEmail(query, filter.Email);

        if (!string.IsNullOrWhiteSpace(filter.PhoneNumber))
            query = _userRepository.FilterPhoneNumber(query, filter.PhoneNumber);
        
        var users = await query.ToListAsync();
        var rolesLookup = await _userRepository.GetRolesForUsersAsync(users);
        if (await IsInRoleAsync(currentUser, "moderator"))
        {
            users = users.Where(u =>
                !rolesLookup.ContainsKey(u.Id) || !rolesLookup[u.Id].Contains("moderator")
            ).ToList();
        }
        
        users = filter.Sort switch
        {
            "date_asc" => users.OrderBy(u => u.Id).ToList(),
            "date_desc" => users.OrderByDescending(u => u.Id).ToList(),
            _ => users.OrderByDescending(u => u.Id).ToList()
        };
        
        users = users.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize).ToList();
        var userDtos = _mapper.Map<List<UserDto>>(users);

        foreach (var dto in userDtos)
        {
            dto.Roles = rolesLookup.ContainsKey(dto.Id) ? rolesLookup[dto.Id] : new List<string>();
        }
        return userDtos;
    }

    public async Task<int> GetFilteredCountAsync(UserFilterDto filter, UserDto? currentUser)
    {
        var query = _userRepository.CreateFilter().Where(u => u.Id != 1);

        if (!string.IsNullOrWhiteSpace(filter.UserName))
            query = _userRepository.FilterUserName(query, filter.UserName);

        if (!string.IsNullOrWhiteSpace(filter.Email))
            query = _userRepository.FilterEmail(query, filter.Email);

        if (!string.IsNullOrWhiteSpace(filter.PhoneNumber))
            query = _userRepository.FilterPhoneNumber(query, filter.PhoneNumber);

        var users = await query.ToListAsync();
        var rolesLookup = await _userRepository.GetRolesForUsersAsync(users);

        if (await IsInRoleAsync(currentUser, "moderator"))
        {
            users = users.Where(u =>
                !rolesLookup.ContainsKey(u.Id) || !rolesLookup[u.Id].Contains("moderator")
            ).ToList();
        }
        return users.Count;
    }

}
