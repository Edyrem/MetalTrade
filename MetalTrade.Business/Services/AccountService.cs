using MetalTrade.Business.Interfaces;
using MetalTrade.DataAccess.Interfaces;
using MetalTrade.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace MetalTrade.Business.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _repository;

        public AccountService(IAccountRepository repository)
        {
            _repository = repository;
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

            return await _repository.CreateUserAsync(user, password);
        }

        public async Task<SignInResult> LoginAsync(string login, string password, bool rememberMe)
        {
            return await _repository.PasswordSignInAsync(login, password, rememberMe);
        }

        public async Task LogoutAsync() => await _repository.SignOutAsync();
    }
}