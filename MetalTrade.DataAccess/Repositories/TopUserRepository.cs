using MetalTrade.DataAccess.Data;
using MetalTrade.DataAccess.Interfaces.Repositories;
using MetalTrade.Domain.Entities;

namespace MetalTrade.DataAccess.Repositories
{
    public class TopUserRepository: Repository<TopUser>, ITopUserRepository
    {
        public TopUserRepository(MetalTradeDbContext context) : base(context)
        {
        }
    }
}
