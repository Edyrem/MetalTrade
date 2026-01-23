using MetalTrade.Web.ViewModels.Advertisement;

namespace MetalTrade.Web.ViewModels.Promotion
{
    public class CommercialViewModel
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public int? CreatedByUserId { get; set; }
        public UserViewModel? CreatedBy { get; set; }
        public int AdvertisementId { get; set; }
        public AdvertisementViewModel Advertisement { get; set; } = null!;
        public decimal Cost { get; set; } = 0;
    }
}
