using MetalTrade.DataAccess.Data;
using MetalTrade.DataAccess.Interfaces.Repositories;
using MetalTrade.Domain.Entities;

namespace MetalTrade.DataAccess.Repositories
{
    public class AdvertisementPhotoRepository : Repository<AdvertisementPhoto>, IAdvertisementPhotoRepository
    {
        public AdvertisementPhotoRepository(MetalTradeDbContext context) : base(context)
        {
        }
    }
}
