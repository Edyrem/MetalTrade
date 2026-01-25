namespace MetalTrade.Web.ViewModels.Chat;

public class ChatListItemViewModel
{
    public int ChatId { get; set; }
    public string Title { get; set; } = "";
    public int UnreadCount { get; set; }

    public string? LastMessageText { get; set; }
    public DateTime? LastMessageDate { get; set; }
    
    public string? Photo { get; set; }
    
    public int? OtherUserId { get; set; }
}
