using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MetalTrade.Models
{
    public class MetalTradeDbContext: IdentityDbContext<User, IdentityRole<int>, int>
    {
    }
}
