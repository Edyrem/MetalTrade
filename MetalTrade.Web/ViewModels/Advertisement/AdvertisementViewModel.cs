using MetalTrade.Web.ViewModels.AdvertisementPhoto;
using MetalTrade.Web.ViewModels.Product;
using System.ComponentModel.DataAnnotations;

namespace MetalTrade.Web.ViewModels.Advertisement;

public class AdvertisementViewModel
{
    public int Id { get; set; }
        
    [Display(Name = "Название")]
    public string Title { get; set; } = string.Empty;
        
    [Display(Name = "Описание")]
    public string Body { get; set; } = string.Empty;
        
    [Display(Name = "Цена")]
    public decimal Price { get; set; }
        
    [Display(Name = "Дата создания")]
    public DateTime CreateDate { get; set; }
        
    [Display(Name = "Адрес")]
    public string? Address { get; set; }
        
    [Display(Name = "Телефон")]
    public string PhoneNumber { get; set; } = string.Empty;
        
    public int ProductId { get; set; }
        
    [Display(Name = "Продукт")]
    public ProductViewModel Product { get; set; } = new();
        
    [Display(Name = "Город")]
    public string? City { get; set; }
        
    public int Status { get; set; }
        
    public bool IsTop { get; set; }
        
    public bool IsAd { get; set; }
        
    public int UserId { get; set; }
        
    public UserViewModel User { get; set; } = new();
        
    public List<AdvertisementPhotoViewModel> Photoes { get; set; } = [];
}