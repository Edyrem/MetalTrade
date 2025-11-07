using MetalTrade.Domain.Entities;

namespace MetalTrade.Business.Dtos
{
    public class AdvertisementDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public decimal Price { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
        public string? Address { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public string? City { get; set; }
        public int Status { get; set; }
        public bool IsTop { get; set; }
        public bool IsAd { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public List<AdvertisementPhoto> Photoes { get; set; } = null!;
    }
}
