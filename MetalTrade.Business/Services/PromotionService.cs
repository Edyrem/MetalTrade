using MetalTrade.Application.Patterns.Strategy.Advertisement.Interfaces;
using MetalTrade.Business.Interfaces;
using MetalTrade.DataAccess.Interfaces.Repositories;
using MetalTrade.Domain.Abstraction;

namespace MetalTrade.Business.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly IPromotionStrategy _strategy;
        private readonly ICommercialRepository _commercialRepository;
        private readonly ITopAdvertisementRepository _topAdvertisementRepository;
        private readonly IAdvertisementRepository _advertisementRepository;
        private readonly ITopUserRepository _topUserRepository;
        private readonly IUserManagerRepository _userRepository;

        public PromotionService(
            IPromotionStrategy strategy,
            ICommercialRepository commercialRepository,
            ITopAdvertisementRepository topAdvertisementRepository,
            IAdvertisementRepository advertisementRepository,
            ITopUserRepository topUserRepository,
            IUserManagerRepository userRepository)
        {
            _strategy = strategy;
            _commercialRepository = commercialRepository;
            _topAdvertisementRepository = topAdvertisementRepository;
            _advertisementRepository = advertisementRepository;
            _topUserRepository = topUserRepository;
            _userRepository = userRepository;
        }

        private async Task<bool> CheckPromotionAsync<T>(T promotion) where T: TimedPromotion
        {
            if (promotion is null) return false;

            var shouldBeActive = await _strategy.ShouldBeActiveAsync(promotion);
            if(promotion.IsActive == shouldBeActive) return false;

            promotion.IsActive = shouldBeActive;
            return true;
        }

        private bool DeactivatePromotion<T>(T promotion) where T : TimedPromotion
        {
            if (promotion is null) return false;
            if (!promotion.IsActive) return false;
            promotion.IsActive = false;
            return true;
        }

        public async Task CreatePromotionAsync(int advertisementId)
        {
            var advertisement = await _advertisementRepository.GetAsync(advertisementId);
            if (advertisement is null) return;

            var changed = false;

            var commercial = await _commercialRepository.GetLast(advertisementId);
            if (await CheckPromotionAsync(commercial))
            {
                changed = true;
                if(!advertisement.IsAd)
                    await _commercialRepository.CreateAsync(commercial);
                advertisement.IsAd = commercial.IsActive;
                await _commercialRepository.UpdateAsync(commercial);
            }

            var topAd = await _topAdvertisementRepository.GetLast(advertisementId);
            if(await CheckPromotionAsync(topAd))
            {
                changed = true;
                if(!advertisement.IsTop)
                    await _topAdvertisementRepository.CreateAsync(topAd);
                else
                    await _topAdvertisementRepository.UpdateAsync(topAd);

                advertisement.IsTop = topAd.IsActive;
            }

            if (changed)
            {
                await _advertisementRepository.UpdateAsync(advertisement);
            }
        }

        public async Task DeactivatePromotionAsync(int advertisementId)
        {
            var advertisement =  await _advertisementRepository.GetAsync(advertisementId);
            if (advertisement is null) return;

            var changed = false;

            var commercial = await _commercialRepository.GetLast(advertisementId);
            if (DeactivatePromotion(commercial))
            {
                changed = true;
                advertisement.IsAd = commercial.IsActive;
                await _commercialRepository.UpdateAsync(commercial);
            }

            var topAd = await _topAdvertisementRepository.GetLast(advertisementId);
            if (DeactivatePromotion(topAd))
            {
                changed = true;
                advertisement.IsTop = topAd.IsActive;
                await _topAdvertisementRepository.UpdateAsync(topAd);
            }

            if (changed)
            {
                await _advertisementRepository.UpdateAsync(advertisement);
            }
        }

        public async Task CreateUserPromotionAsync(int userId)
        {
            var user = await _userRepository.GetAsync(userId);
            if (user is null) return;

            var changed = false;

            var topUser = await _topUserRepository.GetLast(userId);
            if(await CheckPromotionAsync(topUser))
            {
                changed = true;
                if(!user.IsTop)
                    await _topUserRepository.CreateAsync(topUser);
                else
                    await _topUserRepository.UpdateAsync(topUser);
                user.IsTop = topUser.IsActive;
            }

            if (changed)
            {
                await _userRepository.UpdateAsync(user);
            }
        }

        public async Task DeactivateUserPromotionAsync(int userId)
        {
            var user = await _userRepository.GetAsync(userId);
            if (user is null) return;

            var changed = false;

            var topUser = await _topUserRepository.GetAsync(userId);
            if (DeactivatePromotion(topUser))
            {
                changed = true;
                user.IsTop = topUser.IsActive;
                await _topUserRepository.UpdateAsync(topUser);
            }

            if (changed)
            {
                await _userRepository.UpdateAsync(user);
            }
        }
    }
}
