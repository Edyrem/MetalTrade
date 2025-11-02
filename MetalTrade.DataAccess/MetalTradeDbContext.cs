using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MetalTrade.Domain.Entities;

namespace MetalTrade.DataAccess
{
    public class MetalTradeDbContext: IdentityDbContext<User, IdentityRole<int>, int>
    {
    }
}
