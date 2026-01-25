namespace MetalTrade.Domain.Entities;

public class ChatUser
{
    public int ChatId { get; set; }
    public Chat Chat { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public bool IsDeleted { get; set; }
}
