namespace MetalTrade.Domain.Entities;

public class AdvertisementPhoto
{
    public int Id { get; set; }
    public string PhotoLink { get; set; }
    public int AdvertisementId { get; set; }
    public Advertisement? Advertisement { get; set; }
}