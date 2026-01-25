using MetalTrade.Business.Interfaces;
using MetalTrade.DataAccess.Data;
using MetalTrade.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class ChatService : IChatService
{
    private readonly MetalTradeDbContext _context;

    public ChatService(MetalTradeDbContext context)
    {
        _context = context;
    }

    
    public async Task<int> GetOrCreatePrivateChatAsync(int userId1, int userId2)
    {
        var chat = await _context.Chats
            .Include(c => c.Users)
            .FirstOrDefaultAsync(c =>
                c.IsPrivate &&
                c.Users.Any(u => u.UserId == userId1) &&
                c.Users.Any(u => u.UserId == userId2));

        if (chat != null)
        {
            var cu = chat.Users.First(u => u.UserId == userId1);

            if (cu.IsDeleted)
            {
                cu.IsDeleted = false;
                await _context.SaveChangesAsync();
            }

            return chat.Id;
        }


        chat = new Chat
        {
            IsPrivate = true,
            Users = new List<ChatUser>
            {
                new ChatUser { UserId = userId1 },
                new ChatUser { UserId = userId2 }
            }
        };

        _context.Chats.Add(chat);
        await _context.SaveChangesAsync();

        return chat.Id;
    }

}