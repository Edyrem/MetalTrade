namespace MetalTrade.Web.ViewModels.Promotion;

using System.ComponentModel.DataAnnotations;

public class PromotionActivateViewModel
{
    [Required(ErrorMessage = "Введите количество дней")]
    [Range(1, int.MaxValue, ErrorMessage = "Количество дней должно быть больше 0")]
    public int Days { get; set; }

    [Required]
    public int TargetId { get; set; }
}
