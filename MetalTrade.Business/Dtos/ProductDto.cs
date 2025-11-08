using MetalTrade.Domain.Entities;

namespace MetalTrade.Business.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int MetalTypeId { get; set; }
        public MetalType? MetalType { get; set; }
        public List<AdvertisementDto>? Advertisements { get; set; }
    }
}
