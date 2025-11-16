using System.ComponentModel.DataAnnotations;
using MetalTrade.Business.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace MetalTrade.Web.ViewModels.MetalType;

public class CreateMetalViewModel
{
    public int Id { get; set; }
    
    [Display(Name = "Название")]
    [Required(ErrorMessage = "Укажите название металла")]
    [Remote(action:"CheckNameOfMetalType", controller:"Validation", ErrorMessage = "Тип металла с таким названием уже существует!")]
    [MaxLength(20, ErrorMessage = "Можно вводить не более 20 символов")]
    public string Name { get; set; }
    
    public List<ProductDto> Products { get; set; } = [];
}