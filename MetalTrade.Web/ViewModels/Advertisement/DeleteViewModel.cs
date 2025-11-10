using System.ComponentModel.DataAnnotations;

namespace MetalTrade.Web.ViewModels.Advertisement
{
    public class DeleteViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Название")]
        public string Title { get; set; } = string.Empty;
    }
}
