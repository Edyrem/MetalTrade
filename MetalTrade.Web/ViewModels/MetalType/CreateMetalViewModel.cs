using System.ComponentModel.DataAnnotations;
using MetalTrade.Business.Dtos;

namespace MetalTrade.Web.ViewModels.MetalType;

public class CreateMetalViewModel
{
    public int Id { get; set; }
    
    [Display(Name = "Название")]
    [Required(ErrorMessage = "Укажите название металла")]
    [StringLength(20, ErrorMessage = "Можно вводить не более 20 символов")]
    public string Name { get; set; }
    
    public List<ProductDto> Products { get; set; } = [];
}