using MetalTrade.DataAccess.Data;
using MetalTrade.DataAccess.Interfaces.Repositories;
using MetalTrade.Domain.Entities;

namespace MetalTrade.DataAccess.Repositories
{
    public class TopAdvertisementRepository : Repository<TopAdvertisement>, ITopAdvertisementRepository
    {
        public TopAdvertisementRepository(MetalTradeDbContext context) : base(context)
        {
        }
    }
}
