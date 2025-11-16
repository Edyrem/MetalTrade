
namespace MetalTrade.Business.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int MetalTypeId { get; set; }
        public MetalTypeDto? MetalType { get; set; }
        public List<AdvertisementDto>? Advertisements { get; set; }
    }
}
