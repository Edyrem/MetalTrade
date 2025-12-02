using MetalTrade.Business.Dtos;
using MetalTrade.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace MetalTrade.Business.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(int id);
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<bool> AddToRoleAsync(UserDto user, string role);
    Task<bool> IsInRoleAsync(UserDto user, string role);
    Task<bool> RemoveFromRoleAsync(UserDto user, string role);
    Task<bool> CreateUserAsync(UserDto model, string role);
    Task<SignInResult> LoginAsync(string login, string password, bool rememberMe);
    Task LogoutAsync();
    Task<List<UserDto>> GetAllUsersWithRolesAsync();
    Task DeleteUserAsync(int id);
    Task<UserDto?> GetCurrentUserAsync(HttpContext context);
}