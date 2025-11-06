using MetalTrade.Business.Dtos;
using MetalTrade.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace MetalTrade.Business.Interfaces;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<bool> CreateUserAsync(UserDto model, string role);
}