using MetalTrade.DataAccess.Interfaces;
using MetalTrade.Domain.Entities;
using Microsoft.AspNetCore.Identity;
    
namespace MetalTrade.DataAccess.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountRepository(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IdentityResult> CreateUserAsync(User user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
                await _signInManager.SignInAsync(user, isPersistent: false);
            return result;
        }

        public async Task<User?> FindByLoginAsync(string login)
        {
            var user = await _userManager.FindByNameAsync(login);
            if (user == null)
                user = await _userManager.FindByEmailAsync(login);
            return user;
        }

        public async Task<SignInResult> PasswordSignInAsync(string login, string password, bool rememberMe)
        {
            var user = await FindByLoginAsync(login);
            if (user == null)
                return SignInResult.Failed;

            return await _signInManager.PasswordSignInAsync(
                user, password, rememberMe, lockoutOnFailure: false);
        }

        public async Task SignOutAsync() => await _signInManager.SignOutAsync();
    }
}