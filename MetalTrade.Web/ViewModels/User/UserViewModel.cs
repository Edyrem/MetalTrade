using System.ComponentModel.DataAnnotations;

namespace MetalTrade.Web.ViewModels.User
{
    public class UserViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Логин")]
        public string UserName { get; set; }
        [Display(Name = "Почта")]
        public string Email { get; set; }
        [Display(Name = "Номер телефона")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Номер Whatsapp")]
        public string? WhatsAppNumber { get; set; }
        [Display(Name = "Фото профиля")]
        public string? Photo { get; set; }
        [Display(Name = "Роли")]
        public List<string>? Roles { get; set; }
    }
}
