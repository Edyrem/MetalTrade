using MetalTrade.Application.Patterns.Strategy.Advertisement.Interfaces;
using MetalTrade.DataAccess.Interfaces.Repositories;
using MetalTrade.Domain.Abstraction;
using MetalTrade.Domain.Entities;

namespace MetalTrade.Application.Patterns.Strategy.Advertisement.Strategies
{
    public class ViewsBasedPromotionStrategy : IPromotionStrategy
    {
        public string Name => "Views";
        
        private readonly int _minViews;

        public ViewsBasedPromotionStrategy(int minViews = 100)
        {
            _minViews = minViews;
        }

        public async Task<bool> ShouldBeActiveAsync(TimedPromotion timedPromotion)
        {
            if (timedPromotion is not TopAdvertisement topAd)
                return false;
                
            var currentTime = DateTime.UtcNow;
            var isInTimeRange = currentTime >= timedPromotion.StartDate && currentTime <= timedPromotion.EndDate;

            return await Task.FromResult(isInTimeRange
                // && topAd.Advertisement.ViewsCount >= _minViews
                && topAd.Advertisement != null);
        }
    }
}
