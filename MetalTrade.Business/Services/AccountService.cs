using MetalTrade.Business.Interfaces;
using MetalTrade.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace MetalTrade.Business.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountService(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IdentityResult> RegisterAsync(
            string username,
            string email,
            string password,
            string phoneNumber,
            string? whatsappNumber,
            IFormFile? photo)
        {
            var user = new User
            {
                UserName = username,
                Email = email,
                PhoneNumber = phoneNumber,
                WhatsAppNumber = whatsappNumber,
                Photo = "/images/default.png"
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
                await _signInManager.SignInAsync(user, isPersistent: false);

            return result;
        }



        public async Task<SignInResult> LoginAsync(string login, string password, bool rememberMe)
        {
            var user = await _userManager.FindByNameAsync(login)
                       ?? await _userManager.FindByEmailAsync(login);

            if (user == null)
                return SignInResult.Failed;

            return await _signInManager.PasswordSignInAsync(
                user, password, rememberMe, lockoutOnFailure: false);
        }

        public async Task LogoutAsync() =>
            await _signInManager.SignOutAsync();
    }
}