using MetalTrade.DataAccess.Interfaces.Repositories;

namespace MetalTrade.Domain.Entities;

public class AdvertisementPhoto : ISoftDeletable
{
    public int Id { get; set; }
    public string PhotoLink { get; set; }
    public int AdvertisementId { get; set; }
    public Advertisement? Advertisement { get; set; }
    public bool IsDeleted { get; set; }
}