using MetalTrade.Web.ViewModels.Product;
using System.ComponentModel.DataAnnotations;

namespace MetalTrade.Web.ViewModels.Advertisement
{
    public class CreateAdvertisementViewModel
    {
        [Display(Name = "Название")]
        [Required(ErrorMessage = "Поле Название обязательно")]
        [StringLength(200, ErrorMessage = "Можно вводить не более 200 символов")]
        public string Title { get; set; } = string.Empty;
        
        [Display(Name = "Описание")]
        [StringLength(2000, ErrorMessage = "Можно вводить не более 2000 символов")]
        [Required(ErrorMessage = "Поле Описание обязательно")]
        public string? Body { get; set; } = string.Empty;
        
        [Display(Name = "Цена")]
        [Required(ErrorMessage = "Поле Цена объязательно")]
        [Range(1, 1000000, ErrorMessage = "Цена не может быть отрицательной или быть равной нулю!")]
        public decimal Price { get; set; }
        
        [Display(Name = "Адрес")]
        [StringLength(2000, ErrorMessage = "Можно вводить не более 2000 символов")]
        public string? Address { get; set; }
        
        [Display(Name = "Номер телефона")]
        [Required(ErrorMessage = "Поле Номер телефона обязательно")]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "Введите ровно 9 цифр")]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [Display(Name = "Город")]
        [StringLength(200, ErrorMessage = "Можно вводить не более 200 символов")]
        public string? City { get; set; }
        [Display(Name = "Фото")]
        public List<IFormFile>? PhotoFiles { get; set; } = [];
        [Display(Name = "Продукт")]
        [Required(ErrorMessage = "Поле Продукт обязательно")]
        public int ProductId { get; set; }
        public List<ProductViewModel> Products { get; set; } = [];
    }
}
