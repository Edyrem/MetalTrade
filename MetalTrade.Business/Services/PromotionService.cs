using MetalTrade.Application.Patterns.Strategy.Advertisement.Interfaces;
using MetalTrade.Business.Interfaces;
using MetalTrade.DataAccess.Data;
using MetalTrade.DataAccess.Interfaces.Repositories;
using MetalTrade.DataAccess.Repositories;
using MetalTrade.Domain.Abstraction;
using MetalTrade.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace MetalTrade.Business.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly CommercialRepository _commercialRepository;
        private readonly TopAdvertisementRepository _topAdvertisementRepository;
        private readonly AdvertisementRepository _advertisementRepository;
        private readonly TopUserRepository _topUserRepository;
        private readonly UserManagerRepository _userRepository;
        private readonly IPromotionValidator _validator;
        private readonly IPromotionStrategyProvider _strategy;

        private readonly HashSet<Func<Task>> _saveChanger = new();

        public PromotionService(
            MetalTradeDbContext context,
            UserManager<User> userManager,
            IPromotionStrategyProvider strategy,
            IPromotionValidator validator)
        {
            _strategy = strategy;
            _validator = validator;
            _advertisementRepository = new AdvertisementRepository(context);
            _commercialRepository = new CommercialRepository(context);
            _topAdvertisementRepository = new TopAdvertisementRepository(context);
            _topUserRepository = new TopUserRepository(context);
            _userRepository = new UserManagerRepository(context, userManager);
        }

        private async Task<bool> CheckPromotionAsync<T>(T promotion) where T : TimedPromotion
        {
            if (promotion is null) return false;

            var shouldBeActive = await _strategy.GetStrategy<TimedPromotion>().ShouldBeActiveAsync(promotion);
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

            await _validator.ValidateCanActivateasync<Commercial>(ad.Id);

            advertisement.IsActive = true;
            await _commercialRepository.CreateAsync(advertisement);
            RegisterRepoSaves(_commercialRepository.SaveChangesAsync);

            ad.IsAd = true;
            await _advertisementRepository.UpdateAsync(ad);
            RegisterRepoSaves(_advertisementRepository.SaveChangesAsync);      
        }

        public async Task CreateTopAdvertisementAsync(TopAdvertisement advertisement)
        {
            var ad = await _advertisementRepository.GetAsync(advertisement.AdvertisementId);
            if (ad == null)
                throw new ArgumentException($"Advertisement {ad} not found");

            await _validator.ValidateCanActivateasync<TopAdvertisement>(ad.Id);

            advertisement.IsActive = true;
            advertisement.Reason = _strategy.GetStrategy<TopAdvertisement>().Name;
            await _topAdvertisementRepository.CreateAsync(advertisement);
            RegisterRepoSaves(_topAdvertisementRepository.SaveChangesAsync);

            ad.IsTop = true;
            await _advertisementRepository.UpdateAsync(ad);
            RegisterRepoSaves(_advertisementRepository.SaveChangesAsync);
        }

        public async Task CreateTopUserAsync(TopUser topUser)
        {
            var user = await _userRepository.GetAsync(topUser.TargetUserId);
            if (user == null)
                throw new ArgumentException($"Advertisement {user} not found");

            await _validator.ValidateCanActivateasync<TopAdvertisement>(user.Id);

            topUser.IsActive = true;
            topUser.Reason = _strategy.GetStrategy<TopUser>().Name;
            await _topUserRepository.CreateAsync(topUser);
            RegisterRepoSaves(_topUserRepository.SaveChangesAsync);

            user.IsTop = true;
            await _userRepository.UpdateAsync(user);
            RegisterRepoSaves(_userRepository.SaveChangesAsync);
        }

        public async Task UpdatePromotionAsync(int advertisementId)
        {
            var advertisement = await _advertisementRepository.GetAsync(advertisementId);
            if (advertisement is null) return;

            var changed = false;

            var commercial = await _commercialRepository.GetLast(advertisementId);
            if (commercial != null && await CheckPromotionAsync(commercial))
            {
                changed = true;
                advertisement.IsAd = commercial.IsActive;
                await _commercialRepository.UpdateAsync(commercial);
                RegisterRepoSaves(_commercialRepository.SaveChangesAsync);
            }

            var topAd = await _topAdvertisementRepository.GetLast(advertisementId);
            if (topAd != null && await CheckPromotionAsync(topAd))
            {
                changed = true;
                advertisement.IsTop = topAd.IsActive;
                await _topAdvertisementRepository.UpdateAsync(topAd);
                RegisterRepoSaves(_topAdvertisementRepository.SaveChangesAsync);
            }

            if (changed)
            {
                await _advertisementRepository.UpdateAsync(advertisement);
                RegisterRepoSaves(_advertisementRepository.SaveChangesAsync);
            }
        }

        public async Task UpdateUserPromotionAsync(int userId)
        {
            var user = await _userRepository.GetAsync(userId);
            if (user is null) return;

            var changed = false;

            var topUser = await _topUserRepository.GetLast(userId);
            if (topUser != null && await CheckPromotionAsync(topUser))
            {
                changed = true;
                user.IsTop = topUser.IsActive;
                await _topUserRepository.UpdateAsync(topUser);
                RegisterRepoSaves(_topUserRepository.SaveChangesAsync);
            }

            if (changed)
            {
                await _userRepository.UpdateAsync(user);
                RegisterRepoSaves(_userRepository.SaveChangesAsync);
            }
        }

        public async Task DeactivatePromotionAsync(int advertisementId)
        {
            var advertisement = await _advertisementRepository.GetAsync(advertisementId);
            if (advertisement is null) return;

            var changed = false;

            var commercial = await _commercialRepository.GetLast(advertisementId);
            if (commercial != null && DeactivatePromotion(commercial))
            {
                changed = true;
                advertisement.IsAd = commercial.IsActive;
                await _commercialRepository.UpdateAsync(commercial);
                RegisterRepoSaves(_commercialRepository.SaveChangesAsync);
            }

            var topAd = await _topAdvertisementRepository.GetLast(advertisementId);
            if (topAd != null && DeactivatePromotion(topAd))
            {
                changed = true;
                advertisement.IsTop = topAd.IsActive;
                await _topAdvertisementRepository.UpdateAsync(topAd);
                RegisterRepoSaves(_topAdvertisementRepository.SaveChangesAsync);
            }

            if (changed)
            {
                await _advertisementRepository.UpdateAsync(advertisement);
                RegisterRepoSaves(_advertisementRepository.SaveChangesAsync);
            }
        }        

        public async Task DeactivateUserPromotionAsync(int userId)
        {
            var user = await _userRepository.GetAsync(userId);
            if (user is null) return;

            var changed = false;

            var topUser = await _topUserRepository.GetAsync(userId);
            if (topUser != null && DeactivatePromotion(topUser))
            {
                changed = true;
                user.IsTop = topUser.IsActive;
                await _topUserRepository.UpdateAsync(topUser);
                RegisterRepoSaves(_topUserRepository.SaveChangesAsync);
            }

            if (changed)
            {
                await _userRepository.UpdateAsync(user);
                RegisterRepoSaves(_userRepository.SaveChangesAsync);
            }
        }

        public async Task<IEnumerable<Advertisement>> GetAllActiveCommercialsAsync()
        {
            var commercials = await _commercialRepository.GetAllActiveAsync();
            var activeCommercials = new List<Advertisement>();
            foreach (var commercial in commercials)
            {
                if(commercial.EndDate < DateTime.UtcNow)
                {
                    await DeactivatePromotionAsync(commercial.AdvertisementId);
                    continue;
                }
                if(commercial.Advertisement != null)
                    activeCommercials.Add(commercial.Advertisement);
            }
            return activeCommercials;
        }

        public async Task<IEnumerable<Advertisement>> GetAllActiveTopAdvertisementsAsync()
        {
            var topAdvertisements = await _topAdvertisementRepository.GetAllActiveAsync();
            var advertisements = new List<Advertisement>();
            foreach (var topAd in topAdvertisements)
            {
                if (topAd.EndDate < DateTime.UtcNow)
                {
                    await DeactivatePromotionAsync(topAd.AdvertisementId);
                    continue;
                }
                if (topAd.Advertisement != null)
                    advertisements.Add(topAd.Advertisement);
            }
            return advertisements;
        }

        public async Task<IEnumerable<User>> GetAllActiveTopUsersAsync()
        {
            var topUsers = await _topUserRepository.GetAllActiveAsync();
            var users = new List<User>();

            foreach (var user in topUsers)
            {
                if (user.EndDate < DateTime.UtcNow)
                {
                    await DeactivateUserPromotionAsync(user.TargetUserId);
                    continue;
                }
                if (user.TargetUser != null)
                    users.Add(user.TargetUser);
            }
            return users;
        }

        private void RegisterRepoSaves(Func<Task> repoSaver)
        {
            _saveChanger.Add(repoSaver);
        }

        public async Task SaveAllChangesAsync()
        {
            if (_saveChanger.Count > 0)
                foreach (var saver in _saveChanger)
                    await saver();

            _saveChanger.Clear();
        }
    }
}
