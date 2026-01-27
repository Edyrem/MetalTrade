using MetalTrade.Domain.Entities;

namespace MetalTrade.Business.Dtos
{
    public class TopAdvertisementDto
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }

        // ID модератора/администратора, который создал промоушен
        public int? CreatedByUserId { get; set; }
        // Навигационное свойство к модератору (lazy loading)
        public User? CreatedBy { get; set; }
        public int AdvertisementId { get; set; }
        public AdvertisementDto Advertisement { get; set; } = null!;
        public string Reason { get; set; } = string.Empty;
    }
}
