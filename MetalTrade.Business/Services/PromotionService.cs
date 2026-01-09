using MetalTrade.Application.Patterns.Strategy.Advertisement.Interfaces;
using MetalTrade.Business.Interfaces;
using MetalTrade.DataAccess.Interfaces.Repositories;
using MetalTrade.Domain.Abstraction;
using MetalTrade.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private async Task<bool> UpdatePromotionAsync<T>(T promotion) where T: TimedPromotion
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

        public async Task UpdatePromotionAsync(int advertisementId)
        {
            var advertisement = await _advertisementRepository.GetAsync(advertisementId);
            if (advertisement is null) return;

            var changed = false;

            var commercial = await _commercialRepository.GetAsync(advertisementId);
            if(await UpdatePromotionAsync(commercial))
            {
                changed = true;
                advertisement.IsAd = commercial.IsActive;
                await _commercialRepository.UpdateAsync(commercial);
            }

            var topAd = await _topAdvertisementRepository.GetAsync(advertisementId);
            if(await UpdatePromotionAsync(topAd))
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

        public async Task DeactivatePromotionAsync(int advertisementId)
        {
            var advertisement =  await _advertisementRepository.GetAsync(advertisementId);
            if (advertisement is null) return;

            var changed = false;

            var commercial = await _commercialRepository.GetAsync(advertisementId);
            if (DeactivatePromotion(commercial))
            {
                changed = true;
                advertisement.IsAd = commercial.IsActive;
                await _commercialRepository.UpdateAsync(commercial);
            }

            var topAd = await _topAdvertisementRepository.GetAsync(advertisementId);
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

        public async Task UpdateUserPromotionAsync(int userId)
        {
            var user = await _userRepository.GetAsync(userId);
            if (user is null) return;

            var changed = false;

            var topUser = await _topUserRepository.GetAsync(userId);
            if(await UpdatePromotionAsync(topUser))
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
