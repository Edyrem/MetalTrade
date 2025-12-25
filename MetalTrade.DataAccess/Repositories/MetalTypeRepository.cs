using MetalTrade.DataAccess.Data;
using MetalTrade.DataAccess.Interfaces.Repositories;
using MetalTrade.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetalTrade.DataAccess.Repositories
{
    public class MetalTypeRepository : Repository<MetalType>, IMetalTypeRepository
    {
        public MetalTypeRepository(MetalTradeDbContext context) : base(context)
        {
        }
        public IQueryable<MetalType> CreateFilter()
        {
            return _context.MetalTypes.AsQueryable();
        }
    }
}
