using AutoMapper;
using MetalTrade.Business.Dtos;
using MetalTrade.Domain.Entities;

namespace MetalTrade.Business.Common.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Advertisement, AdvertisementDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.PhotoFiles, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Photoes, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());

            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Photo, opt => opt.Ignore()) 
                .ReverseMap()
                .ForMember(dest => dest.Photo, opt => opt.Ignore()); 

            CreateMap<AdvertisementPhotoDto, AdvertisementPhoto>().ReverseMap();

            CreateMap<AdvertisementPhoto, AdvertisementPhotoAjaxDto>();

            CreateMap<TopAdvertisement, TopAdvertisementDto>().ReverseMap();

            CreateMap<Commercial, CommercialDto>().ReverseMap();

            CreateMap<TopUser, TopUserDto>().ReverseMap();

            #region MetalService
            CreateMap<MetalType, MetalTypeDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(dest => dest.Name.ToLower()))
                .ForMember(dest => dest.Products, opt => opt.Ignore())
                .ForMember(dest => dest.Products, opt => opt.Ignore())
                .ReverseMap();
            #endregion

            #region ProductService
            CreateMap<ProductDto, Product>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(dest => dest.Name.ToLower()))
                .ForMember(dest => dest.Advertisements, opt => opt.Ignore())
                .ReverseMap();
            #endregion

            #region UserService
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Photo, opt => opt.Ignore())
                .ForMember(dest => dest.PhotoLink, opt => opt.MapFrom(src => src.Photo))
                .ReverseMap()
                .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => src.PhotoLink));

            CreateMap<TopUser, TopUserDto>().ReverseMap();
            #endregion
            
        }
    }
}