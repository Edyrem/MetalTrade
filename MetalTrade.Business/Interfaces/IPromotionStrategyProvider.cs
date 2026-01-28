
using MetalTrade.Application.Patterns.Strategy.Advertisement.Interfaces;
using MetalTrade.Domain.Abstraction;

namespace MetalTrade.Business.Interfaces
{
    public interface IPromotionStrategyProvider
    {
        IPromotionStrategy GetStrategy<T>() where T : TimedPromotion;
    }
}
