using MetalTrade.Business.Dtos;
using MetalTrade.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace MetalTrade.Business.Interfaces;

public interface IProfileService
{
    Task<ProfileWithAdsDto> GetProfileAsync(User user);

    Task<ProfileDto> GetProfileEditModelAsync(User user);

    Task<bool> UpdateProfileAsync(User user, ProfileDto dto, IFormFile? photo, IWebHostEnvironment env);
}