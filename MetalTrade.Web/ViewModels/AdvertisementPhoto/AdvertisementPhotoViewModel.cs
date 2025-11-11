using MetalTrade.Business.Dtos;

namespace MetalTrade.Web.ViewModels.AdvertisementPhoto
{
    public class AdvertisementPhotoViewModel
    {
        public int Id { get; set; }
        public string PhotoLink { get; set; } = string.Empty;
        public int AdvertisementId { get; set; }
        public AdvertisementDto? Advertisement { get; set; }
    }
}
