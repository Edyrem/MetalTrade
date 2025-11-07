using System.ComponentModel.DataAnnotations;

namespace MetalTrade.Domain.Enums;

public enum UserRole
{
    [Display(Name = "Поставщик")]
    Supplier,
    [Display(Name = "Модератор")]
    Moderator,
    [Display(Name = "Администратор")]
    Admin
}