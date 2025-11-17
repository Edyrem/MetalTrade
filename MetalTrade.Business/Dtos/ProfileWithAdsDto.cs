using MetalTrade.Domain.Entities;

namespace MetalTrade.Business.Dtos;

public class ProfileWithAdsDto : ProfileDto
{
    public bool IsSupplier { get; set; }
    public List<AdvertisementDto>? Advertisements { get; set; }
    public List<AdvertisementDto>? Favorites { get; set; }
}