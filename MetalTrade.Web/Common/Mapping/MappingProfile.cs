using AutoMapper;
using MetalTrade.Business.Dtos;
using MetalTrade.Web.ViewModels;
using MetalTrade.Web.ViewModels.Advertisement;
using MetalTrade.Web.ViewModels.AdvertisementPhoto;
using MetalTrade.Web.ViewModels.MetalType;
using MetalTrade.Web.ViewModels.Product;
using MetalTrade.Web.ViewModels.User;
namespace MetalTrade.Web.Common.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<MetalTypeDto, MetalTypeViewModel>().ReverseMap();
            CreateMap<ProductDto, ProductViewModel>().ReverseMap();

            CreateMap<UserDto, MetalTrade.Web.ViewModels.User.UserViewModel>()
                .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => src.PhotoLink))
                .ReverseMap()
                .ForMember(dest => dest.PhotoLink, opt => opt.MapFrom(src => src.Photo));

            //CreateMap<UserDto, CreateUserViewModel>()
            //    .ForMember(dest => dest.Photo, opt => opt.Ignore())
            //    .ReverseMap()
            //    .ForMember(dest => dest.Photo, opt => opt.Ignore());
            CreateMap<UserDto, CreateUserViewModel>().ReverseMap();

            CreateMap<UserDto, DeleteUserViewModel>()
                .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => src.PhotoLink))
                .ReverseMap()
                .ForMember(dest => dest.PhotoLink, opt => opt.MapFrom(src => src.Photo));

            CreateMap<UserDto, EditUserViewModel>().ReverseMap();

            CreateMap<UserDto, MetalTrade.Web.ViewModels.UserViewModel>()
                .ForMember(dest => dest.Photo, opt => opt.Ignore()) 
                .ReverseMap()
                .ForMember(dest => dest.Photo, opt => opt.Ignore()); 

            CreateMap<AdvertisementPhotoDto, AdvertisementPhotoViewModel>().ReverseMap();
            CreateMap<CreateViewModel, AdvertisementDto>()
                .ForMember(dest => dest.PhotoFiles, opt => opt.MapFrom(src => src.Photoes))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.Product, opt => opt.Ignore());

            CreateMap<AdvertisementDto, AdvertisementViewModel>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.Photoes, opt => opt.MapFrom(src => src.Photoes))
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product))
                .ReverseMap()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.Photoes, opt => opt.MapFrom(src => src.Photoes))
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product));

            CreateMap<AdvertisementDto, EditViewModel>()
                .ReverseMap()
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());

            CreateMap<AdvertisementDto, DeleteViewModel>().ReverseMap();

            CreateMap<CreateViewModel, AdvertisementDto>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.Photoes, opt => opt.Ignore()) 
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());
        }
    }
}
