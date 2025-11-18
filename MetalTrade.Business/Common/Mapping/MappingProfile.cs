using AutoMapper;
using MetalTrade.Business.Dtos;
using MetalTrade.Domain.Entities;

namespace MetalTrade.Business.Common.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AdvertisementDto, Advertisement>()
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Photoes, opt => opt.MapFrom(src => src.Photoes));

            CreateMap<Advertisement, AdvertisementDto>()
                .ForMember(dest => dest.User, opt => opt.Ignore()); 

            CreateMap<AdvertisementPhotoDto, AdvertisementPhoto>().ReverseMap();

            // User - ВООБЩЕ УБИРАЕМ маппинг!
            // CreateMap<User, UserDto>().ReverseMap(); // ЗАКОММЕНТИРОВАТЬ!

            CreateMap<MetalTypeDto, MetalType>().ReverseMap();
            CreateMap<ProductDto, Product>().ReverseMap();
        }
    }
}