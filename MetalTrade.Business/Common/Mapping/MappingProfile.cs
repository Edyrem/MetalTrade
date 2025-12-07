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
                .ForMember(dest => dest.Photoes, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.IsAd, opt => opt.Ignore())
                .ForMember(dest => dest.IsTop, opt => opt.Ignore());

            CreateMap<Advertisement, AdvertisementDto>();

            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Photo, opt => opt.Ignore()) 
                .ReverseMap()
                .ForMember(dest => dest.Photo, opt => opt.Ignore()); 

            CreateMap<AdvertisementPhotoDto, AdvertisementPhoto>().ReverseMap();
            CreateMap<MetalTypeDto, MetalType>().ReverseMap();

            #region MetalService
            CreateMap<MetalTypeDto, MetalType>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(dest => dest.Name.ToLower()))
                .ForMember(dest => dest.Products, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<List<MetalTypeDto>, List<MetalType>>().ReverseMap();
            #endregion

            #region ProductService
            CreateMap<ProductDto, Product>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(dest => dest.Name.ToLower()))
                .ReverseMap();

            CreateMap<List<ProductDto>, List<Product>>()
                .ReverseMap();
            #endregion

            #region UserService
            CreateMap<UserDto, User>().ReverseMap();
            #endregion
        }
    }
}