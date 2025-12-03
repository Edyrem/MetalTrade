using MetalTrade.DataAccess.Interfaces.Repositories;
using Microsoft.AspNetCore.Identity;

namespace MetalTrade.Domain.Entities
{
    public class User: IdentityUser<int>, ISoftDeletable
    {
        public string Photo { get; set; }
        public string WhatsAppNumber { get; set; }
        List<Advertisement> Advertisements { get; set; }
        public bool IsDeleted { get; set; }
    }
}