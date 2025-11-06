using MetalTrade.Business.Dtos;
using MetalTrade.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace MetalTrade.Business.Interfaces;

public interface IAdminService
{
    Task<List<User>> GetAllUsersAsync();

    Task<bool> CreateSupplierAsync(UserDto model);
}