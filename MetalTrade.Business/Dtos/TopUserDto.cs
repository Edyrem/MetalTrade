using MetalTrade.Domain.Entities;

namespace MetalTrade.Business.Dtos
{
    public class TopUserDto
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }

        // ID модератора/администратора, который создал промоушен
        public int? CreatedByUserId { get; set; }
        // Навигационное свойство к модератору (lazy loading)
        public UserDto? CreatedBy { get; set; }
        public int TargetUserId { get; set; }
        public UserDto TargetUser { get; set; } = null!;
        public string Reason { get; set; } = string.Empty;
    }
}
