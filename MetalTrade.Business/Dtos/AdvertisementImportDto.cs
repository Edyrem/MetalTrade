using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetalTrade.Business.Dtos
{
    public class AdvertisementImportDto
    {
        [Required(ErrorMessage = "Заголовок объявления не может быть пустым!")] 
        [StringLength(200, ErrorMessage = "Заголовок должен быть не более 200 символов!")]
        public string Title { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Описание объявления не может быть пустым!")]
        [StringLength(2000, ErrorMessage = "Описание должно быть не более 2000 символов!")]
        public string Body { get; set; } = string.Empty;
        
        [Required]
        [Range(1, 1000000, ErrorMessage = "Цена не может быть отрицательной или быть равной нулю!")]
        public decimal Price { get; set; }
        
        [Required(ErrorMessage = "Номер телефона не может быть пустым!")]
        [MaxLength(9, ErrorMessage = "Допустим только 9-значный номер телефона!")]
        [RegularExpression(@"^[0-9]{9}$", ErrorMessage = "Номер телефона должен состоять из 9 цифр!")]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Продукт не может быть пустым!")]
        public string ProductName { get; set; }= string.Empty;
        
        [StringLength(200, ErrorMessage = "Горолд должен быть не более 200 символов!")]
        public string? City { get; set; }
        
        [StringLength(2000, ErrorMessage = "Адрес должен быть не более 2000 символов!")]
        public string? Address { get; set; }
        
    }
}
