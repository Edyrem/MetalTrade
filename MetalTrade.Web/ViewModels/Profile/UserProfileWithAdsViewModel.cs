using MetalTrade.Business.Dtos;

namespace MetalTrade.Web.ViewModels.Profile
{
    public class UserProfileWithAdsViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? WhatsAppNumber { get; set; }
        public string? PhotoPath { get; set; }

        public bool IsSupplier { get; set; }

        public List<AdvertisementDto>? Advertisements { get; set; }

        /*public List<AdvertisementDto>? Favorites { get; set; }*/

    }
}