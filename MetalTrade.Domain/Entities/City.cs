namespace MetalTrade.Domain.Entities;

public class City
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int RegionId { get; set; }
    public Region? Region { get; set; }
    public List<Advertisement> Advertisements { get; set; }
}