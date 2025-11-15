using MetalTrade.DataAccess.Interfaces.Repositories;

namespace MetalTrade.Domain.Entities;

public class Product : ISoftDeletable
{
    public int Id { get; set; }
    public string Name { get; set; }
   //public int MetalTypeId { get; set; }  с ним работаем
   //public MetalType? MetalType { get; set; }
    public List<Advertisement> Advertisements { get; set; }
    public bool IsDeleted { get; set; }
}