namespace MetalTrade.Web.ViewModels.Commercial;

using System.ComponentModel.DataAnnotations;

public class CommercialViewModel
{
    [Required(ErrorMessage = "Введите количество дней")]
    [Range(1, int.MaxValue, ErrorMessage = "Количество дней должно быть больше 0")]
    public int Days { get; set; }

    [Required]
    public int AdvertisementId { get; set; }
}
