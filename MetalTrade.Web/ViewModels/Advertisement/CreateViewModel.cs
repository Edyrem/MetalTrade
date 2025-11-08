using System.ComponentModel.DataAnnotations;

namespace MetalTrade.Web.ViewModels.Advertisement
{
    public class CreateViewModel
    {
        [Display(Name = "Название")]
        [Required(ErrorMessage = "Поле Название объязательно")]
        [StringLength(200, ErrorMessage = "Можно вводить не более 200 символов")]
        public string? Title { get; set; }
        [Display(Name = "Описание")]
        [StringLength(2000, ErrorMessage = "Можно вводить не более 2000 символов")]
        public string? Body { get; set; }
        [Display(Name = "Цена")]
        [Required(ErrorMessage = "Поле Цена объязательно")]
        public decimal Price { get; set; }
        [Display(Name = "Адрес")]
        [StringLength(2000, ErrorMessage = "Можно вводить не более 2000 символов")]
        public string? Address { get; set; }
        [Display(Name = "Номер телефона")]
        [StringLength(100, ErrorMessage = "Можно вводить не более 100 символов")]
        public string? PhoneNumber { get; set; }
        [Display(Name = "Город")]
        [StringLength(200, ErrorMessage = "Можно вводить не более 200 символов")]
        public string? City { get; set; }
        [Display(Name = "Фото")]
        public IFormFile[]? Images { get; set; }
        [Display(Name = "Продукт")]
        [Required(ErrorMessage = "Поле Продукт объязательно")]
        public int ProductId { get; set; }
    }
}
