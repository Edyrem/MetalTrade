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
                .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber ?? ""))
                .ForMember(dest => dest.Body, opt => opt.MapFrom(src => src.Body ?? ""))
                .ForMember(dest => dest.Photoes, opt => opt.MapFrom(src => src.Photoes));

            CreateMap<Advertisement, AdvertisementDto>();

            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Photo, opt => opt.Ignore()) 
                .ReverseMap()
                .ForMember(dest => dest.Photo, opt => opt.Ignore()); 

            CreateMap<AdvertisementPhotoDto, AdvertisementPhoto>().ReverseMap();
            CreateMap<MetalTypeDto, MetalType>().ReverseMap();
            CreateMap<ProductDto, Product>().ReverseMap();
        }
    }
}