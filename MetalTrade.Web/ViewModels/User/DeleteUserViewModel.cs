using System.ComponentModel.DataAnnotations;

namespace MetalTrade.Web.ViewModels.User
{
    public class DeleteUserViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Логин")]
        public string UserName { get; set; }
        [Display(Name = "Почта")]
        public string Email { get; set; }
        [Display(Name = "Номер телефона")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Номер WhatsApp")]
        public string WhatsAppNumber { get; set; }
        [Display(Name = "Аватар")]
        public string Photo { get; set; }
        [Display(Name = "Статус")]
        public string Role { get; set; }
    }
}
