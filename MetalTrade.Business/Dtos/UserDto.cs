using Microsoft.AspNetCore.Http;

namespace MetalTrade.Business.Dtos;

public class UserDto
{
    public int Id { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber{ get; set ; } = null!;
    public string? WhatsAppNumber{ get; set; }
    public IFormFile? Photo { get; set; }
    public string? PhotoLink { get; set; }
    public string Password { get; set; } = null!;
    public List<string>? Roles { get; set; }
}