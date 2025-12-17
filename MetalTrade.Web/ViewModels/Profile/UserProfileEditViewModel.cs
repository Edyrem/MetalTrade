using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MetalTrade.Web.ViewModels.Profile
{
    public class UserProfileEditViewModel
    {
        public int Id { get; set; }

        [Remote("CheckUserNameEdit", "Validation", AdditionalFields = "Id", ErrorMessage = "Логин уже используется")]
        [Required(ErrorMessage = "Укажите логин")]
        [Display(Name = "Логин")]
        public string UserName { get; set; } = string.Empty;

        [Remote("CheckEmailEdit", "Validation", AdditionalFields = "Id", ErrorMessage = "Email уже используется")]
        [Required(ErrorMessage = "Укажите email")]
        [Display(Name = "Почта")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Введите корректный email")]
        public string Email { get; set; } = string.Empty;

        [Remote("CheckPhoneNumberEdit", "Validation", AdditionalFields = "Id", ErrorMessage = "Телефон уже используется")]
        [Required(ErrorMessage = "Укажите номер телефона")]
        [Display(Name = "Телефон")]
        [RegularExpression(@"^[0-9]{9}$", ErrorMessage = "Номер должен содержать 9 цифр")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Display(Name = "WhatsApp")]
        [RegularExpression(@"^[0-9]{9}$", ErrorMessage = "Номер WhatsApp должен содержать 9 цифр")]
        public string WhatsAppNumber { get; set; }

        [Display(Name = "Аватар")]
        public IFormFile? Photo { get; set; }

        public string? PhotoPath { get; set; }
    }
}