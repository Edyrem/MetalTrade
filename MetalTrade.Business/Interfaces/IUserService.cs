using MetalTrade.Business.Dtos;
using MetalTrade.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace MetalTrade.Business.Interfaces;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<bool> CreateUserAsync(UserDto model, string role);
    Task<bool> RegisterUserAsync(UserDto model);
    Task<SignInResult> LoginAsync(string login, string password, bool rememberMe);
    Task LogoutAsync();
    Task<Dictionary<User, string?>> GetAllUsersWithRolesAsync();
}