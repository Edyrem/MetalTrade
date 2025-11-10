using MetalTrade.Web.ViewModels.AdvertisementPhoto;
using MetalTrade.Web.ViewModels.Product;

namespace MetalTrade.Web.ViewModels.Advertisement
{
    public class AdvertisementViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime CreateDate { get; set; }
        public string? Address { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public int ProductId { get; set; }
        public ProductViewModel Product { get; set; } = new();
        public string? City { get; set; }
        public int Status { get; set; }
        public bool IsTop { get; set; }
        public bool IsAd { get; set; }
        public int UserId { get; set; }
        public UserViewModel User { get; set; } = new();
        public List<AdvertisementPhotoViewModel> Photoes { get; set; } = [];
    }
}
