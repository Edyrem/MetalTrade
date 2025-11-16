using System.Collections.Generic;
using MetalTrade.Domain.Entities;

namespace MetalTrade.Application.ViewModels;

public class UserProfileWithAdsViewModel
{
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string? WhatsAppNumber { get; set; }
    public string? PhotoPath { get; set; }
    public bool IsSupplier { get; set; }

    public List<Domain.Entities.Advertisement>? Advertisements { get; set; }
    
    //для сохраненнх объявлений (еще не реализовано) 
    public List<Domain.Entities.Advertisement>? Favorites { get; set; }
}