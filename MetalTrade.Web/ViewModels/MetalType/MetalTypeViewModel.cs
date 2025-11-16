using MetalTrade.Business.Dtos;

namespace MetalTrade.Web.ViewModels.MetalType
{
    public class MetalTypeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ProductDto> Products { get; set; } = [];
    }
}
