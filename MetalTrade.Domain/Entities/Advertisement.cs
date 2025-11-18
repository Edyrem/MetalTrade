using MetalTrade.DataAccess.Interfaces.Repositories;

namespace MetalTrade.Domain.Entities;

public class Advertisement : ISoftDeletable
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty; 
    public string Body { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    public string? Address { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public string? City { get; set; }
    public int Status { get; set; }
    public bool IsTop { get; set; }
    public bool IsAd { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public List<AdvertisementPhoto> Photoes { get; set; } = null!;
    
    public bool IsDeleted { get; set; }
}