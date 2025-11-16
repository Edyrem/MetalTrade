using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MetalTrade.Web.ViewModels.Product;

public class EditProductViewModel
{
    public int Id { get; set; }
    
    [Display(Name = "Название")]
    [Required(ErrorMessage = "Укажите название продукта")]
    [StringLength(100, ErrorMessage = "Можно вводить не более 100 символов")]
    public string Name { get; set; }
    
    [Display(Name = "Тип металла")]
    [Required(ErrorMessage = "Укажите тип металла")]
    public int MetalTypeId { get; set; }
    
    public List<SelectListItem> MetalTypes { get; set; } = [];
}