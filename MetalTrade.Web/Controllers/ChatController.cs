using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MetalTrade.Business.Interfaces;
using MetalTrade.DataAccess.Data;
using MetalTrade.Domain.Entities;
using MetalTrade.Web.ViewModels.Chat;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class ChatController : Controller
{
    private readonly IChatService _chatService;
    private readonly MetalTradeDbContext _context;

    public ChatController(IChatService chatService, MetalTradeDbContext context)
    {
        _chatService = chatService;
        _context = context;
    }

    // создание чата с владельцем объявления
    [HttpPost]
    public async Task<IActionResult> GetChatId(int advertisementId)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var ad = await _context.Advertisements
            .Where(a => a.Id == advertisementId)
            .Select(a => new { a.UserId, a.Title })
            .FirstAsync();

        var chatId = await _chatService.GetOrCreatePrivateChatAsync(
            currentUserId,
            ad.UserId
        );

        
        var hasMessages = await _context.ChatMessages
            .AnyAsync(m => m.ChatId == chatId);

        if (!hasMessages)
        {
            _context.ChatMessages.Add(new ChatMessage
            {
                ChatId = chatId,
                SenderId = currentUserId,
                Text = $"Здравствуйте, пишу по объявлению «{ad.Title}»",
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
        }

        return Ok(chatId);
    }



    // Получить историю сообщений чата
    [HttpGet]
    public async Task<IActionResult> History(int chatId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var isMember = await _context.ChatUsers
            .AnyAsync(cu => cu.ChatId == chatId && cu.UserId == userId);

        if (!isMember)
            return Forbid();

        var messages = await _context.ChatMessages
            .Where(m => m.ChatId == chatId)
            .OrderBy(m => m.CreatedAt)
            .Select(m => new
            {
                UserName = m.Sender.UserName,
                m.Text,
                m.CreatedAt,
                m.IsRead
            })
            .ToListAsync();

        return Ok(messages);
    }

    

    [Authorize]
    public async Task<IActionResult> Index(int? chatId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        
        ViewBag.ChatList = await _context.Chats
            .Where(c => c.Users.Any(u =>
                u.UserId == userId && !u.IsDeleted))
            .Select(c => new ChatListItemViewModel
            {
                ChatId = c.Id,
                Title = c.IsPrivate
                    ? c.Users.Where(u => u.UserId != userId)
                        .Select(u => u.User.UserName)
                        .FirstOrDefault()
                    : c.Advertisement.Title,

                Photo = c.IsPrivate
                    ? c.Users.Where(u => u.UserId != userId)
                        .Select(u => u.User.Photo)
                        .FirstOrDefault()
                    : null,

                UnreadCount = c.Messages.Count(m =>
                    m.SenderId != userId && !m.IsRead),

                LastMessageText = c.Messages
                    .OrderByDescending(m => m.CreatedAt)
                    .Select(m => m.Text)
                    .FirstOrDefault(),

                LastMessageDate = c.Messages
                    .OrderByDescending(m => m.CreatedAt)
                    .Select(m => m.CreatedAt)
                    .FirstOrDefault()
            })
            .OrderByDescending(c => c.LastMessageDate)
            .ToListAsync();
        
        ViewBag.GlobalUnreadCount = await _context.ChatMessages
            .CountAsync(m =>
                m.SenderId != userId &&
                !m.IsRead &&
                m.Chat.Users.Any(u => u.UserId == userId && !u.IsDeleted)
            );

        
        if (chatId == null)
        {
            ViewBag.ChatId = null;
            return View();
        }
        
        var isMember = await _context.ChatUsers
            .AnyAsync(cu => cu.ChatId == chatId && cu.UserId == userId);

        if (!isMember)
            return Forbid();

        ViewBag.Messages = await _context.ChatMessages
            .Where(m => m.ChatId == chatId)
            .Include(m => m.Sender)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();
        

        var otherUserId = await _context.ChatUsers
            .Where(cu => cu.ChatId == chatId && cu.UserId != userId)
            .Select(cu => cu.UserId)
            .FirstOrDefaultAsync();
        
        var otherUser = await _context.Users
            .Where(u => u.Id == otherUserId)
            .Select(u => new
            {
                u.UserName,
                u.Photo
            })
            .FirstOrDefaultAsync();

        ViewBag.OtherUserName = otherUser?.UserName;
        ViewBag.OtherUserPhoto = otherUser?.Photo;

        
        
        ViewBag.ChatId = chatId;
        ViewBag.OtherUserId = otherUserId;
        ViewBag.CurrentUserId = userId;

        return View();
    }



    // Получить/создать чат с пользователем
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> GetPrivateChatId(int userId)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        if (currentUserId == userId)
            return BadRequest();

        var chatId = await _chatService.GetOrCreatePrivateChatAsync(
            currentUserId,
            userId
        );

        return Ok(chatId);
    }
    
    // Скрыть чат для пользователя
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Delete([FromBody] DeleteChatRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var chatUser = await _context.ChatUsers
            .FirstOrDefaultAsync(cu =>
                cu.ChatId == request.ChatId &&
                cu.UserId == userId);

        if (chatUser == null)
            return Forbid();

        chatUser.IsDeleted = true;

        await _context.SaveChangesAsync();

        return Ok();
    }

    public class DeleteChatRequest
    {
        public int ChatId { get; set; }
    }





}