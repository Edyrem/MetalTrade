using MetalTrade.Application.Patterns.Strategy.Advertisement.Interfaces;
using MetalTrade.Application.Patterns.Strategy.Advertisement.Strategies;
using MetalTrade.Business.Interfaces;
using MetalTrade.Domain.Abstraction;
using MetalTrade.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace MetalTrade.Business.Helpers
{
    public class PromotionStrategyProvider: IPromotionStrategyProvider
    {
        private readonly IPromotionStrategy _commercialStrategy;
        private readonly IPromotionStrategy _topAdvertisementStrategy;
        private readonly IPromotionStrategy _topUserStrategy;

        public PromotionStrategyProvider(IConfiguration configuration)
        {
            string commercialStrategyType = configuration.GetValue<string>("Promotion:Strategy:Commercial") ?? "TimeBased";
            string topAdvertisementStrategyType = configuration.GetValue<string>("Promotion:Strategy:TopAdvertisement") ?? "TimeBased";
            string topUserStrategyType = configuration.GetValue<string>("Promotion:Strategy:TopUser") ?? "TimeBased";
            
            int topAdvertisementMinViews = configuration.GetValue<int>("Promotion:MinViews");
            decimal topUserMinRating = configuration.GetValue<decimal>("Promotion:MinRating");

            _commercialStrategy = CreateCommertialStrategy(commercialStrategyType);
            _topAdvertisementStrategy = CreateTopAdvertisementStrategy(topAdvertisementStrategyType, topAdvertisementMinViews);
            _topUserStrategy = CreateTopUserStrategy(topUserStrategyType, topUserMinRating);
        }

        private IPromotionStrategy CreateCommertialStrategy(string strategyType)
        {
            return strategyType switch
            {
                "TimeBased" => new TimeBasedPromotionStrategy(),
                _ => new TimeBasedPromotionStrategy()
            };
        }
        private IPromotionStrategy CreateTopAdvertisementStrategy(string strategyType, int minViews)
        {
            return strategyType switch
            {
                "TimeBased" => new TimeBasedPromotionStrategy(),
                "ViewsBased" => new ViewsBasedPromotionStrategy(minViews),
                _ => new TimeBasedPromotionStrategy()
            };
        }
        private IPromotionStrategy CreateTopUserStrategy(string strategyType, decimal minRating)
        {
            return strategyType switch
            {
                "TimeBased" => new TimeBasedPromotionStrategy(),
                "RatingBased" => new RatingBasedPromotionStrategy(minRating),
                _ => new TimeBasedPromotionStrategy()
            };
        }

        public IPromotionStrategy GetStrategy<T>() where T : TimedPromotion
        {
            return typeof(T) switch
            {
                var t when t == typeof(Commercial) => _commercialStrategy,
                var t when t == typeof(TopAdvertisement) => _topAdvertisementStrategy,
                var t when t == typeof(TopUser) => _topUserStrategy,
                _ => throw new NotSupportedException($"Unknown promotion type: {typeof(T).Name}")
            };
        }

    }
}
