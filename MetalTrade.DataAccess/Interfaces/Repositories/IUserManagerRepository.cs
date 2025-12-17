using MetalTrade.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetalTrade.DataAccess.Interfaces.Repositories
{
    public interface IUserManagerRepository: IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByUserNameAsync(string userName);
        Task<User?> GetWithAdvertisementsAsync(int id);
        Task<IdentityResult> CreateAsync(User user, string password);
        Task<IdentityResult> AddToRoleAsync(User user, string role);
        Task<IdentityResult> RemoveFromRoleAsync(User user, string role);
        Task<IEnumerable<string>?> GetUserRolesAsync(User user);
        Task<bool> IsInRoleAsync(User user, string role);
        Task<User?> GetCurrentUserAsync(HttpContext context);
        Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword);
    }
}
