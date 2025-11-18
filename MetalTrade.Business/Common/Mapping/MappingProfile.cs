using AutoMapper;
using MetalTrade.Business.Dtos;
using MetalTrade.Domain.Entities;

namespace MetalTrade.Business.Common.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Advertisement - полный маппинг
            CreateMap<AdvertisementDto, Advertisement>()
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Photoes, opt => opt.MapFrom(src => src.Photoes));

            CreateMap<Advertisement, AdvertisementDto>();

            // User - маппим все поля КРОМЕ Photo
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Photo, opt => opt.Ignore()) // Игнорируем только Photo
                .ReverseMap()
                .ForMember(dest => dest.Photo, opt => opt.Ignore()); // Игнорируем только Photo

            CreateMap<AdvertisementPhotoDto, AdvertisementPhoto>().ReverseMap();
            CreateMap<MetalTypeDto, MetalType>().ReverseMap();
            CreateMap<ProductDto, Product>().ReverseMap();
        }
    }
}