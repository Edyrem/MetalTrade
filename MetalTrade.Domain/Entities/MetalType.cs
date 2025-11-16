using MetalTrade.DataAccess.Interfaces.Repositories;

namespace MetalTrade.Domain.Entities;

public class MetalType  : ISoftDeletable
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Product> Products { get; set; }
    public bool IsDeleted { get; set; }
}