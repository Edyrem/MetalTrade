using MetalTrade.DataAccess.Abstractions;
using MetalTrade.DataAccess.Data;
using MetalTrade.DataAccess.Interfaces.Repositories;
using MetalTrade.Domain.Entities;
using Microsoft.AspNetCore.Http;
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

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _dbSet.Include(x => x.TopUsers.Where(t => t.IsActive)).FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetWithAdvertisementsAsync(int id)
        {
            return await _dbSet
                .Include(u => u.Advertisements.Where(a => !a.IsDeleted))
                .ThenInclude(p => p.Product)
                .ThenInclude(c => c.MetalType)
                .Include(u => u.Advertisements.Where(a => !a.IsDeleted))
                .ThenInclude(a => a.Photoes.Where(p => !p.IsDeleted))
                .Include(u => u.TopUsers.Where(t => t.IsActive))
                .FirstOrDefaultAsync(u => u.Id == id);
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

        public async Task<IdentityResult> RemoveFromRoleAsync(User user, string role)
        {
            return await _userManager.RemoveFromRoleAsync(user, role);
        }

        public async Task<IEnumerable<string>?> GetUserRolesAsync(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return roles;
        }

        public async Task<bool> IsInRoleAsync(User user, string role)
        {
            return await _userManager.IsInRoleAsync(user, role);
        }

        public async Task<User?> GetCurrentUserAsync(HttpContext context) => await _userManager.GetUserAsync(context.User);

        public async Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }
        
        public IQueryable<User> CreateFilter()
        {
            return _context.Users.AsQueryable();
        }
        
        public async Task<Dictionary<int, List<string>>> GetRolesForUsersAsync(List<User> users)
        {
            var userIds = users.Select(u => u.Id).ToList();

            // Получаем все роли для этих пользователей
            var roles = await _context.UserRoles
                .Where(ur => userIds.Contains(ur.UserId))
                .Join(_context.Roles,
                    ur => ur.RoleId,
                    r => r.Id,
                    (ur, r) => new { ur.UserId, r.Name })
                .ToListAsync();

            // Группируем по UserId
            var rolesLookup = roles
                .GroupBy(r => r.UserId)
                .ToDictionary(g => g.Key, g => g.Select(r => r.Name).ToList());

            return rolesLookup;
        }

        public IQueryable<User> FilterUserName(IQueryable<User> query, string userName)
        {
            return query.Where(u => u.UserName != null && u.UserName.ToLower().Contains(userName.ToLower()));
        }

        public IQueryable<User> FilterEmail(IQueryable<User> query, string email)
        {
            return query.Where(u => u.Email != null && u.Email.Contains(email));
        }

        public IQueryable<User> FilterPhoneNumber(IQueryable<User> query, string phoneNumber)
        {
            return query.Where(u => u.PhoneNumber != null && u.PhoneNumber.Contains(phoneNumber));
        }

    }
}
