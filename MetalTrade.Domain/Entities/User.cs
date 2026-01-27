using MetalTrade.DataAccess.Interfaces.Repositories;
using Microsoft.AspNetCore.Identity;

namespace MetalTrade.Domain.Entities
{
    public class User: IdentityUser<int>, ISoftDeletable
    {
        public string Photo { get; set; }
        public string? WhatsAppNumber { get; set; }
        public List<Advertisement> Advertisements { get; set; } = null!;
        public bool IsDeleted { get; set; }
        public bool IsTop { get; set; } = false;
        public ICollection<TopUser> TopUsers { get; set; } = new List<TopUser>();
        public DateTime? LastSeen { get; set; }
    }
}