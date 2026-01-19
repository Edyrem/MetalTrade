using MetalTrade.Domain.Abstraction;

namespace MetalTrade.Business.Interfaces
{
    public interface IPromotionValidator
    {
        Task ValidateCanActivateasync<T>(int entityId) where T : TimedPromotion;
    }
}
