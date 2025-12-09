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
                .ForMember(dest => dest.PhotoFiles, opt => opt.Ignore())
                //.ForMember(dest => dest.CreateDate, opt => opt.Ignore())
                //.ForMember(dest => dest.Product, opt => opt.Ignore())
                //.ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber ?? ""))
                .ForMember(dest => dest.Body, opt => opt.MapFrom(src => src.Body ?? ""))
                //.ForMember(dest => dest.Photoes, opt => opt.Ignore())
                //.ForMember(dest => dest.Status, opt => opt.Ignore())
                //.ForMember(dest => dest.IsAd, opt => opt.Ignore())
                //.ForMember(dest => dest.IsTop, opt => opt.Ignore())                
                .ReverseMap()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.ProductId, opt => opt.Ignore())
                .ForMember(d => d.Product, opt => opt.Ignore());

            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Photo, opt => opt.Ignore()) 
                .ReverseMap()
                .ForMember(dest => dest.Photo, opt => opt.Ignore()); 

            CreateMap<AdvertisementPhotoDto, AdvertisementPhoto>().ReverseMap();

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
                .ReverseMap();
            #endregion
        }
    }
}