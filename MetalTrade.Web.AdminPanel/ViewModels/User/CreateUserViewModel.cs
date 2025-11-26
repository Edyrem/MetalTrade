using System.ComponentModel.DataAnnotations;
using MetalTrade.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace MetalTrade.Web.AdminPanel.ViewModels.User;

public class CreateUserViewModel
{
    [Remote(action:"CheckUserName", controller:"Validation", ErrorMessage = "Аккаунт с таким именем уже существует!")]
    [Required(ErrorMessage = "Укажите логин")]
    [Display(Name = "Логин")]
    public string UserName { get; set; }
    
    [Remote(action:"CheckEmail", controller:"Validation", ErrorMessage = "Аккаунт с таким Email уже существует!")]
    [Required(ErrorMessage = "Укажите email")]
    [EmailAddress(ErrorMessage = "Введите корректный email")]
    [Display(Name = "Почта")]
    public string Email { get; set; }
    
    [Remote(action:"CheckPhoneNumber", controller:"Validation", ErrorMessage = "Аккаунт с таким номером телефона уже существует!")]
    [Required(ErrorMessage = "Укажите ваш номер телефона")]
    [Display(Name = "Номер телефона")]
    [DataType(DataType.PhoneNumber)]
    [RegularExpression(@"^\(?([0-9]{4})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{3})$", ErrorMessage = "Некорректный ввод номера телефона. Необходимо 10 цифр")]
    public string PhoneNumber { get; set; }
    
    [Remote(action:"CheckWhatsAppNumber", controller:"Validation", ErrorMessage = "Аккаунт с таким номером WhatsApp уже существует!")]
    [Required(ErrorMessage = "Укажите номер WhatsApp")]
    [Display(Name = "Номер WhatsApp")]
    [DataType(DataType.PhoneNumber)]
    [RegularExpression(@"^\(?([0-9]{4})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{3})$", ErrorMessage = "Некорректный ввод номера WhatsApp. Необходимо 10 цифр")]
    public string WhatsAppNumber { get; set; }
    
    [Required(ErrorMessage = "Загрузите картинку для аватара профиля")]
    [Display(Name = "Аватар")]
    public IFormFile Photo { get; set; }
    
    [Required(ErrorMessage = "Укажите пароль")]
    [DataType(DataType.Password)]
    [StringLength(20, MinimumLength = 6, ErrorMessage = "Пароль должен содержать минимум 6 символов")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "Пароль должен содержать хотя бы 1 заглавную, 1 строчную букву и 1 цифру")]
    [Display(Name = "Пароль")]
    public string Password { get; set; }
    
    [Required(ErrorMessage = "Подтверждение пароля")]
    [Compare("Password", ErrorMessage = "Пароли не совпадают")]
    [DataType(DataType.Password)]
    [Display(Name = "Подтвердить пароль")]
    public string PasswordConfirm { get; set; }
    
    [Required(ErrorMessage = "Укажите роль")]
    [Display(Name = "Роль")]
    public UserRole Role { get; set; }
}