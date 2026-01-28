namespace MetalTrade.Web.ViewModels.Promotion
{
    public class TopUserViewModel
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }

        // ID модератора/администратора, который создал промоушен
        public int? CreatedByUserId { get; set; }
        // Навигационное свойство к модератору (lazy loading)
        public UserViewModel? CreatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public int TargetUserId { get; set; }
        public UserViewModel TargetUser { get; set; } = null!;
        public string Reason { get; set; } = string.Empty;
    }
}
