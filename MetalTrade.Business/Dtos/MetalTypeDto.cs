
namespace MetalTrade.Business.Dtos
{
    public class MetalTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<ProductDto>? Products { get; set; }
    }
}
