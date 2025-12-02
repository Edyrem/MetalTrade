using System.ComponentModel.DataAnnotations;

namespace MetalTrade.Web.ViewModels.Advertisement
{
    public class DeleteAdvertisementViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Название")]
        public string Title { get; set; } = string.Empty;
        public int UserId { get; set; }
    }
}
