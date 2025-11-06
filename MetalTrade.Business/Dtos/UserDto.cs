using Microsoft.AspNetCore.Http;

namespace MetalTrade.Business.Dtos;

public class UserDto
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber{ get; set ; }
    public string? WhatsAppNumber{ get; set; }
    public IFormFile? Photo { get; set; }
    public string Password{ get; set; }
}