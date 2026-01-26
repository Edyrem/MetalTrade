namespace MetalTrade.Domain.Entities;

public class ChatMessage
{
    public int Id { get; set; }

    public int ChatId { get; set; }
    public Chat Chat { get; set; } = null!;

    public int SenderId { get; set; }
    public User Sender { get; set; } = null!;

    public bool IsRead { get; set; } = false;

    public string Text { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}