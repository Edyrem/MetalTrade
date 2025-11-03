namespace MetalTrade.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int MetalTypeId { get; set; }
    public MetalType? MetalType { get; set; }
    public List<Advertisement> Advertisements { get; set; }
}