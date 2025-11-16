using MetalTrade.Application.ViewModels;
using MetalTrade.Domain.Entities;
using Microsoft.AspNetCore.Hosting;

namespace MetalTrade.Business.Interfaces;

public interface IProfileService
{
    Task<UserProfileWithAdsViewModel> GetProfileAsync(User user);
    Task<bool> UpdateProfileAsync(User user, UserProfileViewModel model, IWebHostEnvironment env);
    Task<UserProfileViewModel> GetProfileEditModelAsync(User user);
}

