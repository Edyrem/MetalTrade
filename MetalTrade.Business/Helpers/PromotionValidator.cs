using MetalTrade.DataAccess.Interfaces.Repositories;
using MetalTrade.Business.Interfaces;
using MetalTrade.Domain.Abstraction;
using MetalTrade.Domain.Entities;
using MetalTrade.Domain.Exceptions;

namespace MetalTrade.Business.Helpers
{
    public class PromotionValidator: IPromotionValidator
    {
        private readonly ICommercialRepository _commercialRepository;
        private readonly ITopAdvertisementRepository _topAdvertisementRepository;
        private readonly ITopUserRepository _topUserRepository;

        public PromotionValidator(
            ICommercialRepository commercialRepository,
            ITopAdvertisementRepository topAdvertisementRepository,
            ITopUserRepository topUserRepository)
        {
            _commercialRepository = commercialRepository;
            _topAdvertisementRepository = topAdvertisementRepository;
            _topUserRepository = topUserRepository;
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
