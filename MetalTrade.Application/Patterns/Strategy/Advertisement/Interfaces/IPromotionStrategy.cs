

using MetalTrade.Domain.Abstraction;

namespace MetalTrade.Application.Patterns.Strategy.Advertisement.Interfaces
{
    public interface IPromotionStrategy
    {
        public string Name { get; }
        public Task<bool> ShouldBeActiveAsync(TimedPromotion timedPromotion);
    }
}
