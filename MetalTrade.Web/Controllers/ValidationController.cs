using MetalTrade.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;

namespace MetalTrade.Web.Controllers;

public class ValidationController : Controller
{
    private MetalTradeDbContext _context;

    public ValidationController(MetalTradeDbContext context)
    {
        _context = context;
    }
    
    [AcceptVerbs("GET", "POST")]
    public bool CheckUserName(string userName)
    {
        return !_context.Users.Any(u => u.UserName.ToLower() == userName.ToLower() );
    }
    
    [AcceptVerbs("GET", "POST")]
    public bool CheckEmail(string email)
    {
        return !_context.Users.Any(u =>  u.Email == email );
    }
    
    [AcceptVerbs("GET", "POST")]
    public bool CheckPhoneNumber(string phoneNumber)
    {
        return !_context.Users.Any(u =>  u.PhoneNumber == phoneNumber );
    }
    
    [AcceptVerbs("GET", "POST")]
    public bool CheckWhatsAppNumber(string whatsAppNumber)
    {
        return !_context.Users.Any(u =>  u.WhatsAppNumber == whatsAppNumber );
    }
}