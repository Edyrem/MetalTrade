namespace MetalTrade.Domain.Entities;

public class Chat
{
    public int Id { get; set; }
    public bool IsPrivate { get; set; }
    public int? AdvertisementId { get; set; }
    public Advertisement? Advertisement { get; set; }

    public ICollection<ChatUser> Users { get; set; } = new List<ChatUser>();
    public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}