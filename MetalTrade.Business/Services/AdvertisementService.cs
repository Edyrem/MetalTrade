using MetalTrade.Business.Dtos;
using MetalTrade.Business.Interfaces;
using MetalTrade.DataAccess.Data;
using MetalTrade.DataAccess.Repositories;
using MetalTrade.Domain.Entities;

namespace MetalTrade.Business.Services
{
    public class AdvertisementService
    {
        private readonly AdvertisementRepository _repository;
        public AdvertisementService(MetalTradeDbContext context)
        {
            _repository = new AdvertisementRepository(context);
        }

    }
}
