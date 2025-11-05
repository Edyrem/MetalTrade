using MetalTrade.DataAccess.Data;
using MetalTrade.DataAccess.Interfaces.Repositories;
using MetalTrade.DataAccess.Repositories;
using MetalTrade.Domain.Entities;

namespace MetalTrade.DataAccess;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(MetalTradeDbContext context) : base(context)
    {
    }    
}