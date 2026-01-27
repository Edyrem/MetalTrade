using MetalTrade.Web.ViewModels.Advertisement;

namespace MetalTrade.Web.ViewModels.Promotion
{
    public class TopAdvertisementViewModel
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }

        // ID модератора/администратора, который создал промоушен
        public int? CreatedByUserId { get; set; }
        // Навигационное свойство к модератору (lazy loading)
        public UserViewModel? CreatedBy { get; set; }
        public int AdvertisementId { get; set; }
        public AdvertisementViewModel Advertisement { get; set; } = null!;
        public string Reason { get; set; } = string.Empty;
    }
}
