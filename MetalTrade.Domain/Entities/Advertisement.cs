namespace MetalTrade.Domain.Entities;

public class Advertisement
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public decimal Price { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    public string? Address { get; set; }

    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public int CityId { get; set; }
    public City? City { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public List<AdvertisementPhoto> Photos { get; set; }
    public List<AdvertisementProductProperty> ProductProperties { get; set; }
}