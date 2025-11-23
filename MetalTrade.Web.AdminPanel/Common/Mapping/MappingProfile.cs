using AutoMapper;
using MetalTrade.Business.Dtos;
using MetalTrade.Web.AdminPanel.ViewModels;
//using MetalTrade.Web.AdminPanel.ViewModels.Advertisement;
//using MetalTrade.Web.ViewModels.AdvertisementPhoto;
//using MetalTrade.Web.ViewModels.MetalType;
//using MetalTrade.Web.ViewModels.Product;

namespace MetalTrade.Web.AdminPanel.Common.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //CreateMap<MetalTypeDto, MetalTypeViewModel>().ReverseMap();
            //CreateMap<ProductDto, ProductViewModel>().ReverseMap();

            //CreateMap<UserDto, UserViewModel>()
            //    .ForMember(dest => dest.Photo, opt => opt.Ignore()) 
            //    .ReverseMap()
            //    .ForMember(dest => dest.Photo, opt => opt.Ignore()); 

            //CreateMap<AdvertisementPhotoDto, AdvertisementPhotoViewModel>().ReverseMap();
            //CreateMap<CreateViewModel, AdvertisementDto>()
            //    .ForMember(dest => dest.PhotoFiles, opt => opt.MapFrom(src => src.Photoes))
            //    .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            //    .ForMember(dest => dest.Product, opt => opt.Ignore());

            //CreateMap<AdvertisementDto, AdvertisementViewModel>()
            //    .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
            //    .ForMember(dest => dest.Photoes, opt => opt.MapFrom(src => src.Photoes))
            //    .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product))
            //    .ReverseMap()
            //    .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
            //    .ForMember(dest => dest.Photoes, opt => opt.MapFrom(src => src.Photoes))
            //    .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product));

            //CreateMap<AdvertisementDto, EditViewModel>()
            //    .ReverseMap()
            //    .ForMember(dest => dest.Product, opt => opt.Ignore())
            //    .ForMember(dest => dest.User, opt => opt.Ignore());

            //CreateMap<AdvertisementDto, DeleteViewModel>().ReverseMap();

            //CreateMap<CreateViewModel, AdvertisementDto>()
            //    .ForMember(dest => dest.UserId, opt => opt.Ignore())
            //    .ForMember(dest => dest.Photoes, opt => opt.Ignore()) 
            //    .ForMember(dest => dest.Product, opt => opt.Ignore())
            //    .ForMember(dest => dest.User, opt => opt.Ignore());
        }
    }
}
