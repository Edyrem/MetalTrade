
namespace MetalTrade.Business.Dtos
{
    public class MetalTypeDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public List<ProductDto>? Products { get; set; }
    }
}
