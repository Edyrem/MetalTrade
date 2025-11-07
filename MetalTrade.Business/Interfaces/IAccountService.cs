using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace MetalTrade.Business.Interfaces
{
    public interface IAccountService
    {
        Task<IdentityResult> RegisterAsync(
            string username,
            string email,
            string password,
            string phoneNumber,
            string? whatsAppNumber,
            IFormFile? photo);

        Task<SignInResult> LoginAsync(
            string login, 
            string password, 
            bool rememberMe);
        Task LogoutAsync();
    }
}
