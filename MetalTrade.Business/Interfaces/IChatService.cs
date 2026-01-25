namespace MetalTrade.Business.Interfaces;

public interface IChatService
{
    Task<int> GetOrCreatePrivateChatAsync(int userId1, int userId2);

}
