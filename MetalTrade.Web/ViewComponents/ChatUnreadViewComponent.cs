using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using MetalTrade.DataAccess.Data;


public class ChatUnreadViewComponent : ViewComponent
{
    private readonly MetalTradeDbContext _context;


    public ChatUnreadViewComponent(MetalTradeDbContext context)
    {
        _context = context;
    }


    // Подсчет непрочитанных сообщений
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userId = int.Parse(
            HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value
        );


        var count = await _context.ChatMessages
            .Where(m =>
                m.SenderId != userId &&
                !m.IsRead &&
                _context.ChatUsers.Any(cu =>
                    cu.ChatId == m.ChatId &&
                    cu.UserId == userId &&
                    !cu.IsDeleted
                )
            )
            .CountAsync();




        return View(count);
    }

}