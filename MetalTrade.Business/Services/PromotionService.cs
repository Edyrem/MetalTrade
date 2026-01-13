using MetalTrade.Application.Patterns.Strategy.Advertisement.Interfaces;
using MetalTrade.Business.Interfaces;
using MetalTrade.DataAccess.Interfaces.Repositories;
using MetalTrade.Domain.Abstraction;
using MetalTrade.Domain.Entities;

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

        private async Task<bool> CheckPromotionAsync<T>(T promotion) where T : TimedPromotion
        {
            if (promotion is null) return false;

            var shouldBeActive = await _strategy.ShouldBeActiveAsync(promotion);
            if (promotion.IsActive == shouldBeActive) return false;

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

        public async Task CreateCommercialAdvertisementAsync(Commercial advertisement)
        {
            var ad = await _advertisementRepository.GetAsync(advertisement.AdvertisementId);
            if (ad == null)
                throw new ArgumentException($"Advertisement {ad} not found");

            advertisement.IsActive = true;
            await _commercialRepository.CreateAsync(advertisement);
            ad.IsAd = true;
            await _advertisementRepository.UpdateAsync(ad);
        }

        public async Task CreateTopAdvertisementAsync(TopAdvertisement advertisement)
        {
            var ad = await _advertisementRepository.GetAsync(advertisement.AdvertisementId);
            if (ad == null)
                throw new ArgumentException($"Advertisement {ad} not found");

            advertisement.IsActive = true;
            advertisement.Reason = _strategy.Name;
            await _topAdvertisementRepository.CreateAsync(advertisement);
            ad.IsTop = true;
            await _advertisementRepository.UpdateAsync(ad);
        }

        public async Task CreateTopUserAsync(TopUser topUser)
        {
            var user = await _userRepository.GetAsync(topUser.UserId);
            if (user == null)
                throw new ArgumentException($"Advertisement {user} not found");

            topUser.IsActive = true;
            topUser.Reason = _strategy.Name;
            await _topUserRepository.CreateAsync(topUser);

            user.IsTop = true;
            await _userRepository.UpdateAsync(user);
        }

        public async Task UpdatePromotionAsync(int advertisementId)
        {
            var advertisement = await _advertisementRepository.GetAsync(advertisementId);
            if (advertisement is null) return;

            var changed = false;

            var commercial = await _commercialRepository.GetLast(advertisementId);
            if (await CheckPromotionAsync(commercial))
            {
                changed = true;
                advertisement.IsAd = commercial.IsActive;
                await _commercialRepository.UpdateAsync(commercial);
            }

            var topAd = await _topAdvertisementRepository.GetLast(advertisementId);
            if (await CheckPromotionAsync(topAd))
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

            var topUser = await _topUserRepository.GetLast(userId);
            if (await CheckPromotionAsync(topUser))
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

        public async Task DeactivatePromotionAsync(int advertisementId)
        {
            var advertisement = await _advertisementRepository.GetAsync(advertisementId);
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

        public async Task SaveCommercialAdvertisementAsync()
        {
            await _commercialRepository.SaveChangesAsync();
        }

        public async Task SaveTopAdvertisementAsync()
        {
            await _topAdvertisementRepository.SaveChangesAsync();
        }

        public async Task SaveTopUserAsync()
        {
            await _topUserRepository.SaveChangesAsync();
        }
    }
}
