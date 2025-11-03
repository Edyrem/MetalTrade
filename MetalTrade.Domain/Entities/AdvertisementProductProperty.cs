namespace MetalTrade.Domain.Entities;

public class AdvertisementProductProperty
{
    public int Id { get; set; }
    public float Size { get; set; }

    public int PropertyId { get; set; }
    public ProductProperty? Property { get; set; }
    public int AdvertisementId { get; set; }
    public Advertisement? Advertisement { get; set; }
}