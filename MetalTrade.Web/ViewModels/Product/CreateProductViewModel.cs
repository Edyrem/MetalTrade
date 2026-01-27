using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace MetalTrade.Web.ViewModels.Product;

public class CreateProductViewModel
{
    public int Id { get; set; }
    
    [Display(Name = "Название")]
    [Required(ErrorMessage = "Укажите название продукта")]
    [Remote(action:"CheckNameOfProduct", controller:"Validation", ErrorMessage = "Продукт с таким названием уже существует!")]
    [MaxLength(100, ErrorMessage = "Можно вводить не более 100 символов")]
    public string Name { get; set; }
    
    [Display(Name = "Тип материала")]
    [Required(ErrorMessage = "Укажите тип материала")]
    public int MetalTypeId { get; set; }
}