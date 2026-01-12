using MetalTrade.Application.Patterns.Strategy.Advertisement.Interfaces;
using MetalTrade.DataAccess.Interfaces.Repositories;
using MetalTrade.Domain.Abstraction;
using MetalTrade.Domain.Entities;

namespace MetalTrade.Application.Patterns.Strategy.Advertisement.Strategies
{
    public class RatingBasedPromotionStrategy : IPromotionStrategy
    {
        public string Name => "RatingBased";

        private readonly int _minRating;

        public RatingBasedPromotionStrategy(IUserManagerRepository userRepository, int minViews = 100)
        {
            _minRating = minViews;
        }

        public async Task<bool> ShouldBeActiveAsync(TimedPromotion timedPromotion)
        {
            if (timedPromotion is not TopUser topUser)
                return false;

            var currentTime = DateTime.UtcNow;
            var isInTimeRange = currentTime >= timedPromotion.StartDate && currentTime <= timedPromotion.EndDate;

            return await Task.FromResult(isInTimeRange
                // && topUser.User.Rating >= _minRating
                && topUser.User != null);
        }
    }
}
