using Microsoft.AspNetCore.Identity;

namespace MetalTrade.Domain.Entities
{
    public class User: IdentityUser<int>
    {
        public string Photo { get; set; }
        public string? WhatsAppNumber { get; set; }
        List<Advertisement> Advertisements { get; set; }
    }
}