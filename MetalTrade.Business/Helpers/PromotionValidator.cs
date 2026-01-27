using MetalTrade.Business.Interfaces;
using MetalTrade.DataAccess.Data;
using MetalTrade.DataAccess.Interfaces.Repositories;
using MetalTrade.DataAccess.Repositories;
using MetalTrade.Domain.Abstraction;
using MetalTrade.Domain.Entities;
using MetalTrade.Domain.Exceptions;

namespace MetalTrade.Business.Helpers
{
    public class PromotionValidator: IPromotionValidator
    {
        private readonly CommercialRepository _commercialRepository;
        private readonly TopAdvertisementRepository _topAdvertisementRepository;
        private readonly TopUserRepository _topUserRepository;

        public PromotionValidator(
            MetalTradeDbContext context)
        {
            _commercialRepository = new CommercialRepository(context);
            _topAdvertisementRepository = new TopAdvertisementRepository(context);
            _topUserRepository = new TopUserRepository(context);
        }

        public async Task ValidateCanActivateasync<T>(int entityId) where T : TimedPromotion
        {
            bool isActive = typeof(T) switch
            {
                var t when t == typeof(Commercial) => await _commercialRepository.HasActiveAsync(entityId),
                var t when t == typeof(TopAdvertisement) => await _topAdvertisementRepository.HasActiveAsync(entityId),
                var t when t == typeof(TopUser) => await _topUserRepository.HasActiveAsync(entityId),
                _ => throw new NotSupportedException($"Promotion type '{typeof(T).Name}' is not supported for validation.")
            };
            if (isActive)
            {
                throw new PromotionAlreadyActiveException(entityId, typeof(T).Name);
            }
        }
    }
}
