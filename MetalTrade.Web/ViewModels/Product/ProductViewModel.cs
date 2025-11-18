using MetalTrade.Web.ViewModels.MetalType;

namespace MetalTrade.Web.ViewModels.Product
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MetalTypeId { get; set; }
        public MetalTypeViewModel MetalType { get; set; } = new();
    }
}
