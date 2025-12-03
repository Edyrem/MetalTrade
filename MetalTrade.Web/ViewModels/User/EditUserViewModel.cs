using System.ComponentModel.DataAnnotations;
using MetalTrade.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace MetalTrade.Web.ViewModels.User;

public class EditUserViewModel
{
    public int Id { get; set; }

    [Remote("CheckUserNameEdit", "Validation", AdditionalFields = "Id", ErrorMessage = "Логин уже используется")]
    [Required(ErrorMessage = "Укажите логин")]
    [Display(Name = "Логин")]
    public string UserName { get; set; } = string.Empty;

    [Remote("CheckEmailEdit", "Validation", AdditionalFields = "Id", ErrorMessage = "Email уже используется")]
    [Required(ErrorMessage = "Укажите email")]
    [EmailAddress(ErrorMessage = "Введите корректный email")]
    [Display(Name = "Почта")]
    public string Email { get; set; }

    [Remote("CheckPhoneNumberEdit", "Validation", AdditionalFields = "Id", ErrorMessage = "Телефон уже используется")]
    [Required(ErrorMessage = "Укажите ваш номер телефона")]
    [Display(Name = "Номер телефона")]
    [DataType(DataType.PhoneNumber)]
    [RegularExpression(@"^\(?([0-9]{4})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{3})$", ErrorMessage = "Некорректный ввод номера телефона. Необходимо 10 цифр")]
    public string PhoneNumber { get; set; }
    
    [Remote(action: "CheckWhatsappNumberEdit", controller:"Validation", AdditionalFields = "Id", ErrorMessage = "Аккаунт с таким номером WhatsApp уже существует!")]
    [Required(ErrorMessage = "Укажите номер WhatsApp")]
    [Display(Name = "Номер WhatsApp")]
    [DataType(DataType.PhoneNumber)]
    [RegularExpression(@"^\(?([0-9]{4})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{3})$", ErrorMessage = "Некорректный ввод номера WhatsApp. Необходимо 10 цифр")]
    public string WhatsAppNumber { get; set; }
    
    [Required(ErrorMessage = "Загрузите картинку для аватара профиля")]
    [Display(Name = "Аватар")]
    public IFormFile Photo { get; set; }
    
    [Required(ErrorMessage = "Укажите роль")]
    [Display(Name = "Роль")]
    public UserRole Role { get; set; }
}