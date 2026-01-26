using MetalTrade.DataAccess.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using MetalTrade.Domain.Entities;

namespace MetalTrade.Web.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly MetalTradeDbContext _context;


        private static readonly Dictionary<int, HashSet<string>> OnlineUsers = new();
        private static readonly object _lock = new();

        private static readonly Dictionary<int, DateTime> LastMessageTime = new();
        private static readonly object _spamLock = new();


        public ChatHub(MetalTradeDbContext context)
        {
            _context = context;
        }

        public Task<List<int>> GetOnlineUsers()
        {
            lock (_lock)
            {
                return Task.FromResult(OnlineUsers.Keys.ToList());
            }
        }

        // Получить время последнего онлайна пользователя
        public async Task<DateTime?> GetLastSeen(int userId)
        {
            return await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => u.LastSeen)
                .FirstOrDefaultAsync();
        }

        // Подключение пользователя к хабу
        public override async Task OnConnectedAsync()
        {
            var userId = int.Parse(Context.User!
                .FindFirstValue(ClaimTypes.NameIdentifier)!);

            lock (_lock)
            {
                if (!OnlineUsers.ContainsKey(userId))
                    OnlineUsers[userId] = new HashSet<string>();

                OnlineUsers[userId].Add(Context.ConnectionId);
            }

            await Clients.All.SendAsync("UserOnline", userId);
            await base.OnConnectedAsync();
        }
        // Отключение пользователя от хаба
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = int.Parse(Context.User!
                .FindFirstValue(ClaimTypes.NameIdentifier)!);

            var isOffline = false;

            lock (_lock)
            {
                if (OnlineUsers.TryGetValue(userId, out var connections))
                {
                    connections.Remove(Context.ConnectionId);

                    if (connections.Count == 0)
                    {
                        OnlineUsers.Remove(userId);
                        isOffline = true;
                    }
                }
            }

            if (isOffline)
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    user.LastSeen = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }

                await Clients.All.SendAsync("UserOffline", userId);
            }


            await base.OnDisconnectedAsync(exception);
        }

        // Подписаться на конкретный чат
        public async Task JoinChat(int chatId)
        {
            var userId = int.Parse(Context.User!
                .FindFirstValue(ClaimTypes.NameIdentifier)!);

            var isMember = await _context.ChatUsers
                .AnyAsync(cu => cu.ChatId == chatId && cu.UserId == userId);

            if (!isMember)
                throw new HubException("Access denied");

            await Groups.AddToGroupAsync(Context.ConnectionId, $"chat-{chatId}");
            await Groups.AddToGroupAsync(Context.ConnectionId, $"chat-list-{userId}");
        }

        // Отправить сообщение в чат
        public async Task<long> SendMessage(int chatId, string text)
        {
            var userId = int.Parse(Context.User!
                .FindFirstValue(ClaimTypes.NameIdentifier)!);

            var now = DateTime.UtcNow;
            lock (_spamLock)
            {
                if (LastMessageTime.TryGetValue(userId, out var last) &&
                    (now - last).TotalMilliseconds < 700)
                    throw new HubException("Too fast");

                LastMessageTime[userId] = now;
            }

            text = text?.Trim();
            if (string.IsNullOrEmpty(text))
                throw new HubException("Empty");

            if (text.Length > 5000)
                throw new HubException("Too long");

            var message = new ChatMessage
            {
                ChatId = chatId,
                SenderId = userId,
                Text = text,
                IsRead = false
            };

            _context.ChatMessages.Add(message);
            var receiverChatUser = await _context.ChatUsers
                .FirstOrDefaultAsync(cu =>
                    cu.ChatId == chatId &&
                    cu.UserId != userId);

            if (receiverChatUser != null && receiverChatUser.IsDeleted)
            {
                receiverChatUser.IsDeleted = false;
            }


            await _context.SaveChangesAsync();

            var deletedUsers = await _context.ChatUsers
                .Where(cu => cu.ChatId == chatId && cu.IsDeleted)
                .ToListAsync();

            if (deletedUsers.Count > 0)
            {
                foreach (var cu in deletedUsers)
                    cu.IsDeleted = false;

                await _context.SaveChangesAsync();
            }

            var userIds = await _context.ChatUsers
                .Where(cu => cu.ChatId == chatId)
                .Select(cu => cu.UserId)
                .ToListAsync();
            var userName = Context.User!.Identity!.Name!;

            foreach (var uid in userIds)
            {
                await Clients.Group($"chat-list-{uid}")
                    .SendAsync("ChatUpdated", chatId, userName, text);
            }




            await Clients.Group($"chat-{chatId}")
                .SendAsync("ReceiveMessage", userName, text, message.CreatedAt);

            return message.Id;
        }


        // Отметить сообщения как прочитанные
        public async Task MarkAsRead(int chatId)
        {
            var userId = int.Parse(Context.User!
                .FindFirstValue(ClaimTypes.NameIdentifier)!);

            await _context.ChatMessages
                .Where(m => m.ChatId == chatId &&
                            m.SenderId != userId &&
                            !m.IsRead)
                .ExecuteUpdateAsync(s =>
                    s.SetProperty(m => m.IsRead, true));
            var receivers = await _context.ChatUsers
                .Where(cu => cu.ChatId == chatId && cu.UserId != userId)
                .Select(cu => cu.UserId)
                .ToListAsync();


            var isAnyReceiverOnline = receivers.Any(r =>
                OnlineUsers.ContainsKey(r));
            


            var senderIds = await _context.ChatMessages
                .Where(m => m.ChatId == chatId &&
                            m.IsRead &&
                            m.SenderId != userId)
                .Select(m => m.SenderId)
                .Distinct()
                .ToListAsync();

            foreach (var senderId in senderIds)
            {
                await Clients.Group($"chat-list-{senderId}")
                    .SendAsync("MessagesRead", chatId);
            }

            await Clients.Group($"chat-list-{userId}")
                .SendAsync("ChatRead", chatId);
        }

        // Уведомить о наборе текста
        public async Task Typing(int chatId)
        {
            var userName = Context.User!.Identity!.Name!;
            await Clients.GroupExcept($"chat-{chatId}", Context.ConnectionId)
                .SendAsync("Typing", userName);
        }

        public async Task JoinChatList()
        {
            var userId = int.Parse(Context.User!
                .FindFirstValue(ClaimTypes.NameIdentifier)!);

            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                $"chat-list-{userId}"
            );
        }

    }
}