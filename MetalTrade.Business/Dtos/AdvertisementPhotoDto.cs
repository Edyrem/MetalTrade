
namespace MetalTrade.Business.Dtos
{
    public class AdvertisementPhotoDto
    {
        public int Id { get; set; }
        public string PhotoLink { get; set; }
        public int AdvertisementId { get; set; }
        public AdvertisementDto? Advertisement { get; set; }
    }
}
