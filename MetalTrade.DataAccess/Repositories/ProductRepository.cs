using System.Linq.Expressions;
using MetalTrade.DataAccess.Data;
using MetalTrade.DataAccess.Interfaces.Repositories;
using MetalTrade.DataAccess.Repositories;
using MetalTrade.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MetalTrade.DataAccess;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(MetalTradeDbContext context) : base(context)
    {
    }    
    
    public override async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _dbSet.Include(p => p.MetalType)
            .Include(p => p.Advertisements).ToListAsync();
    }
    public override async Task<Product?> GetAsync(int id)
    {
        return await _dbSet.Include(p => p.MetalType)
            .Include(p => p.Advertisements)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
    public override async Task<IEnumerable<Product>> FindAsync(Expression<Func<Product, bool>> predicate)
    {
        return await _dbSet.Include(p => p.MetalType)
            .Include(p => p.Advertisements).Where(predicate).ToListAsync();
    }
}