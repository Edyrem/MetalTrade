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
        [Required] 
        [StringLength(200, ErrorMessage = "Можно вводить не более 200 символов")]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [StringLength(2000, ErrorMessage = "Можно вводить не более 2000 символов")]
        public string Body { get; set; } = string.Empty;
        
        [Required]
        [Range(1, 1000000, ErrorMessage = "Цена не может быть отрицательной или быть равной нулю!")]
        public decimal Price { get; set; }
        
        [Required]
        [MaxLength(9)]
        [RegularExpression(@"^[0-9]{9}$")]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [Required]
        public string ProductName { get; set; }= string.Empty;
        
        [StringLength(200, ErrorMessage = "Можно вводить не более 200 символов")]
        public string? City { get; set; }
        
        [StringLength(2000, ErrorMessage = "Можно вводить не более 2000 символов")]
        public string? Address { get; set; }
        
        //public string Unit { get; set; }
    }
}
