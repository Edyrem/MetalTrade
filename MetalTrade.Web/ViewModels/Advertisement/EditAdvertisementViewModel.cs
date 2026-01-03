using MetalTrade.Web.ViewModels.Product;
using System.ComponentModel.DataAnnotations;

namespace MetalTrade.Web.ViewModels.Advertisement
{
    public class EditAdvertisementViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Название")]
        [Required(ErrorMessage = "Поле Название объязательно")]
        [StringLength(200, ErrorMessage = "Можно вводить не более 200 символов")]
        public string Title { get; set; } = string.Empty;
        [Display(Name = "Описание")]
        [StringLength(2000, ErrorMessage = "Можно вводить не более 2000 символов")]
        [Required(ErrorMessage = "Поле Описание объязательно")]
        public string Body { get; set; } = string.Empty;
        [Display(Name = "Цена")]
        [Required(ErrorMessage = "Поле Цена объязательно")]
        public decimal Price { get; set; }
        [Display(Name = "Адрес")]
        [StringLength(2000, ErrorMessage = "Можно вводить не более 2000 символов")]
        public string? Address { get; set; }
        [Display(Name = "Номер телефона")]
        [StringLength(100, ErrorMessage = "Можно вводить не более 100 символов")]
        [Required(ErrorMessage = "Поле Номер телефона объязательно")]
        public string PhoneNumber { get; set; } = string.Empty;
        [Display(Name = "Город")]
        [StringLength(200, ErrorMessage = "Можно вводить не более 200 символов")]
        public string? City { get; set; }
        [Display(Name = "Продукт")]
        [Required(ErrorMessage = "Поле Продукт объязательно")]
        public int ProductId { get; set; }
        public List<ProductViewModel> Products { get; set; } = [];
        public int UserId { get; set; }
    }
}
