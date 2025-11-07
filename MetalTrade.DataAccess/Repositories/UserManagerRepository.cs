using MetalTrade.DataAccess.Data;
using MetalTrade.DataAccess.Interfaces.Repositories;
using MetalTrade.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MetalTrade.DataAccess.Repositories
{
    public class UserManagerRepository : Repository<User>, IUserManagerRepository
    {
        private readonly UserManager<User> _userManager;
        public UserManagerRepository(MetalTradeDbContext context, UserManager<User> userManager) : base(context)
        {
            _userManager = userManager;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByUserNameAsync(string userName)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public override async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _dbSet.Skip(1).ToListAsync();
        }

        public async Task<IdentityResult> CreateAsync(User user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<IdentityResult> AddToRoleAsync(User user, string role)
        {
            return await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<string?> GetUserRoleAsync(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return roles.FirstOrDefault();
        }
        
    }
}
