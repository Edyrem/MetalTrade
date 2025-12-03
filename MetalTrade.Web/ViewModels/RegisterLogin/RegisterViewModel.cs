using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MetalTrade.Web.ViewModels
{
    public class RegisterViewModel
    {
        [Remote(action: "CheckUserName", controller: "Validation", ErrorMessage = "Аккаунт с таким именем уже существует!")]
        [Required(ErrorMessage = "Укажите логин")]
        [Display(Name = "Логин")]
        public string UserName { get; set; }


        [Remote(action: "CheckEmail", controller: "Validation", ErrorMessage = "Аккаунт с таким Email уже существует!")]
        [Required(ErrorMessage = "Укажите email")]
        [EmailAddress(ErrorMessage = "Введите корректный email")]
        [Display(Name = "Почта")]
        public string Email { get; set; }


        [Remote(action: "CheckPhoneNumber", controller: "Validation", ErrorMessage = "Аккаунт с таким номером телефона уже существует!")]
        [Required(ErrorMessage = "Укажите ваш номер телефона")]
        [Display(Name = "Номер телефона")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^[0-9]{9}$", ErrorMessage = "Номер должен содержать ровно 9 цифр")]
        public string PhoneNumber { get; set; }
        
        [Required(ErrorMessage = "Укажите номер WhatsApp")]
        [RegularExpression(@"^[0-9]{9}$", ErrorMessage = "Номер WhatsApp должен содержать 9 цифр")]
        public string WhatsAppNumber { get; set; }

        
        public IFormFile? Photo { get; set; }


        [Required(ErrorMessage = "Укажите пароль")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Пароль должен содержать минимум 6 символов")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "Пароль должен содержать 1 заглавную, 1 строчную букву и 1 цифру")]
        [Display(Name = "Пароль")]
        public string Password { get; set; }


        [Required(ErrorMessage = "Подтвердите пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердить пароль")]
        public string PasswordConfirm { get; set; }
    }
}
