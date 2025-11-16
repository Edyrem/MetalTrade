using System.ComponentModel.DataAnnotations;

namespace MetalTrade.Web.ViewModels.MetalType;

public class DeleteMetalViewModel
{
    public int Id { get; set; }
    
    [Display(Name = "Название")]
    public string Name { get; set; } = string.Empty;
}