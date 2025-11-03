namespace MetalTrade.Domain.Entities;

public class MetalType
{
    public int Id { get; set; }
    public string Name { get; set; }
    private List<Product> Products { get; set; }
}