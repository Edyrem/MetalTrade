namespace MetalTrade.Business.Dtos;

public class ProfileDto
{
    public int Id { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string WhatsAppNumber { get; set; }
    public string? PhotoPath { get; set; }
}