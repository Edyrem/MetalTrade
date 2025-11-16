using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MetalTrade.Application.ViewModels;

public class UserProfileViewModel
{
    public int Id { get; set; }

    [Remote(action: "CheckUserNameEdit", controller: "Validation", AdditionalFields = "Id", ErrorMessage = "Аккаунт с таким именем уже существует!")]
    [Required(ErrorMessage = "Укажите логин")]
    [Display(Name = "Логин")]
    public string UserName { get; set; } = null!;

    [Remote(action: "CheckEmailEdit", controller: "Validation", AdditionalFields = "Id", ErrorMessage = "Аккаунт с таким Email уже существует!")]
    [Required(ErrorMessage = "Укажите email")]
    [Display(Name = "Почта")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Введите корректный email")]
    public string Email { get; set; } = null!;

    [Remote(action: "CheckPhoneNumberEdit", controller: "Validation", AdditionalFields = "Id", ErrorMessage = "Аккаунт с таким номером телефона уже существует!")]
    [Required(ErrorMessage = "Укажите ваш номер телефона")]
    [Display(Name = "Номер телефона")]
    [DataType(DataType.PhoneNumber)]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Номер должен содержать ровно 10 цифр")]
    [StringLength(10, MinimumLength = 10, ErrorMessage = "Номер должен содержать 10 цифр")]
    public string PhoneNumber { get; set; } = null!;


    [Display(Name = "Номер WhatsApp")]
    [DataType(DataType.PhoneNumber)]
    [RegularExpression(@"^\(?([0-9]{4})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{3})$", ErrorMessage = "Некорректный ввод номера WhatsApp. Необходимо 10 цифр")]
    public string? WhatsAppNumber { get; set; }

    [Display(Name = "Аватар")]
    public IFormFile? Photo { get; set; }

    public string? PhotoPath { get; set; }
    
    
    [DataType(DataType.Password)]
    [Display(Name = "Старый пароль")]
    public string? OldPassword { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Новый пароль")]
    [StringLength(20, MinimumLength = 6, ErrorMessage = "Минимум 6 символов")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "Пароль должен содержать заглавную, строчную букву и цифру")]
    public string? NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Подтвердите пароль")]
    [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
    public string? ConfirmPassword { get; set; }

    
}
