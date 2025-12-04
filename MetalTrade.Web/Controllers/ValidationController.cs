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
        var digits = new string((phoneNumber ?? "").Where(char.IsDigit).ToArray());
        if (digits.Length != 9) return false;
        return !_context.Users.Any(u => u.PhoneNumber == digits);
    }
    
    [AcceptVerbs("GET", "POST")]
    public bool CheckNameOfMetalType(string name)
    {
        return !_context.MetalTypes.Any(u => u.Name.ToLower() == name.ToLower() );
    }
    
    
    [AcceptVerbs("GET", "POST")]
    public bool CheckNameOfProduct(string name)
    {
        return !_context.Products.Any(u => u.Name.ToLower() == name.ToLower() );
    }
    
    
    [AcceptVerbs("GET", "POST")]
    public IActionResult CheckUserNameEdit(string userName, int id)
    {
        bool exists = _context.Users.Any(u =>
            u.UserName.ToLower() == userName.ToLower() &&
            u.Id != id);
        return Json(!exists);
    }
    
    [AcceptVerbs("GET", "POST")]
    public IActionResult CheckEmailEdit(string email, int id)
    {
        bool exists = _context.Users.Any(u =>
            u.Email.ToLower() == email.ToLower() &&
            u.Id != id);
        return Json(!exists);
    }
    
    [AcceptVerbs("GET", "POST")]
    public IActionResult CheckPhoneNumberEdit(string phoneNumber, int id)
    {
        var normalized = new string((phoneNumber ?? "").Where(char.IsDigit).ToArray());
        bool exists = _context.Users.Any(u =>
            u.PhoneNumber == normalized &&
            u.Id != id);
        return Json(!exists);
    }

    [AcceptVerbs("GET", "POST")]
    public IActionResult CheckWhatsAppNumber(string whatsAppNumber)
    {
        var digits = new string((whatsAppNumber ?? "").Where(char.IsDigit).ToArray());
        if (digits.Length != 9) return Json(false);

        bool exists = _context.Users.Any(u => u.WhatsAppNumber == digits);
        return Json(!exists);
    }


    [AcceptVerbs("GET", "POST")]
    public IActionResult CheckWhatsappNumberEdit(string whatsAppNumber, int id)
    {
        var digits = new string((whatsAppNumber ?? "").Where(char.IsDigit).ToArray());
        if (digits.Length != 9) return Json(false);

        bool exists = _context.Users.Any(u => u.WhatsAppNumber == digits && u.Id != id);
        return Json(!exists);
    }
    
}