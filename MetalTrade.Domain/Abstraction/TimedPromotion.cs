
using MetalTrade.Domain.Entities;

namespace MetalTrade.Domain.Abstraction
{
    public abstract class TimedPromotion
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }

        // ID модератора/администратора, который создал промоушен
        public int? CreatedByUserId { get; set; }
        // Навигационное свойство к модератору (lazy loading)
        public User? CreatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
