using MetalTrade.Application.Patterns.Strategy.Advertisement.Interfaces;
using MetalTrade.Business.Interfaces;
using MetalTrade.DataAccess.Interfaces.Repositories;
using MetalTrade.Domain.Abstraction;
using MetalTrade.Domain.Entities;
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

        private readonly HashSet<Func<Task>> _saveChanger = new();

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

            advertisement.IsActive = true;
            advertisement.Reason = _strategy.Name;
            await _topAdvertisementRepository.CreateAsync(advertisement);
            RegisterRepoSaves(_topAdvertisementRepository.SaveChangesAsync);
            ad.IsTop = true;
            await _advertisementRepository.UpdateAsync(ad);
            RegisterRepoSaves(_advertisementRepository.SaveChangesAsync);
        }

        public async Task CreateTopUserAsync(TopUser topUser)
        {
            var user = await _userRepository.GetAsync(topUser.UserId);
            if (user == null)
                throw new ArgumentException($"Advertisement {user} not found");

            topUser.IsActive = true;
            topUser.Reason = _strategy.Name;
            await _topUserRepository.CreateAsync(topUser);
            RegisterRepoSaves(_topUserRepository.SaveChangesAsync);

            user.IsTop = true;
            await _userRepository.UpdateAsync(user);
            RegisterRepoSaves(_userRepository.SaveChangesAsync);
        }

        public async Task<Advertisement?> UpdatePromotionAsync(int advertisementId)
        {
            var advertisement = await _advertisementRepository.GetAsync(advertisementId);
            if (advertisement is null) return null;

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
                return advertisement;
            }
            return null;
        }

        public async Task<User?> UpdateUserPromotionAsync(int userId)
        {
            var user = await _userRepository.GetAsync(userId);
            if (user is null) return null;

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
                return user;
            }

            return null;
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

        public async Task<List<Advertisement>> GetAllActiveCommercialsAsync()
        {
            var commercials = await _commercialRepository.GetAllActiveAsync();
            var advertisements = new List<Advertisement>();
            foreach (var comm in commercials)
            {
                var ad = await UpdatePromotionAsync(comm.AdvertisementId);
                if (ad != null)
                    advertisements.Add(ad);
            }

            await SaveAllChangesAsync();
            return advertisements.Where(x => x.IsAd).ToList();
        }

        public async Task<List<Advertisement>> GetAllActiveTopAdvertisementsAsync()
        {
            var topAdvertisements = await _topAdvertisementRepository.GetAllActiveAsync();
            var advertisements = new List<Advertisement>();

            foreach (var topAdvertisement in topAdvertisements)
            {
                var ad = await UpdatePromotionAsync(topAdvertisement.AdvertisementId);
                if(ad != null)
                    advertisements.Add(ad);
            }

            await SaveAllChangesAsync();
            return advertisements.Where(x => x.IsTop).ToList();
        }

        public async Task<List<User>> GetAllActiveTopUsersAsync()
        {
            var topUsers = await _topUserRepository.GetAllActiveAsync();
            var users = new List<User>();
            foreach (var tUser in topUsers)
            {
                var user = await UpdateUserPromotionAsync(tUser.UserId);
                if(user != null)
                    users.Add(user);
            }
            await SaveAllChangesAsync();
            return users.Where(x => x.IsTop).ToList();
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
