using MetalTrade.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace MetalTrade.DataAccess.Interfaces
{
    public interface IAccountRepository
    {
        Task<IdentityResult> CreateUserAsync(User user, string password);
        Task<SignInResult> PasswordSignInAsync(string login, string password, bool rememberMe);
        Task SignOutAsync();
        Task<User?> FindByLoginAsync(string login);
    }
}