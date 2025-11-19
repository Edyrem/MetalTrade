
using Microsoft.AspNetCore.Http;

namespace MetalTrade.Business.Dtos
{
    public class AdvertisementDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty; 
        public string Body { get; set; } 
        public decimal Price { get; set; }
        public DateTime CreateDate { get; set; }
        public string? Address { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public ProductDto Product { get; set; } = new();
        public string? City { get; set; }
        public int Status { get; set; }
        public bool IsTop { get; set; }
        public bool IsAd { get; set; }
        public int UserId { get; set; }
        public UserDto? User { get; set; }
        public List<AdvertisementPhotoDto> Photoes { get; set; } = [];
        public List<IFormFile>? PhotoFiles { get; set; } 
    }
}
