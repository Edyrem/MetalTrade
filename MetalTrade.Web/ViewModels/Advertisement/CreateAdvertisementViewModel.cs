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
        [Required(ErrorMessage = "Поле Цена обязательно")]
        public decimal Price { get; set; }
        
        [Display(Name = "Адрес")]
        [StringLength(2000, ErrorMessage = "Можно вводить не более 2000 символов")]
        public string? Address { get; set; }
        
        [Display(Name = "Номер телефона")]
        [MaxLength(9, ErrorMessage = "Максимальное количество цифр- 9")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^[0-9]{9}$",
            ErrorMessage = "Некорректный ввод номера телефона. Введите 9 цифр. <br/> Пример : (555) (555) (555)")]
        [Required(ErrorMessage = "Поле Номер телефона обязательно")]
        public string? PhoneNumber { get; set; } = string.Empty;
        
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
