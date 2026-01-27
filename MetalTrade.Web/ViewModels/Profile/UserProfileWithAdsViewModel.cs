using MetalTrade.Business.Dtos;
using MetalTrade.Web.ViewModels.Advertisement;
using MetalTrade.Web.ViewModels.Promotion;

namespace MetalTrade.Web.ViewModels.Profile
{
    public class UserProfileWithAdsViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string WhatsAppNumber { get; set; }
        public string? PhotoPath { get; set; }
        public bool IsTop { get; set; }

        public bool IsSupplier { get; set; }
        public List<string> Roles { get; set; } = null!;
        public List<AdvertisementViewModel>? Advertisements { get; set; }
        public List<TopUserViewModel> TopUsers { get; set; } = [];
        public int UserId { get; set; }

        /*public List<AdvertisementDto>? Favorites { get; set; }*/

    }
}