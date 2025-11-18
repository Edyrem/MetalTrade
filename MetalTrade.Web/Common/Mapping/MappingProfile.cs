using AutoMapper;
using MetalTrade.Business.Dtos;
using MetalTrade.Web.ViewModels;
using MetalTrade.Web.ViewModels.Advertisement;
using MetalTrade.Web.ViewModels.AdvertisementPhoto;
using MetalTrade.Web.ViewModels.MetalType;
using MetalTrade.Web.ViewModels.Product;
namespace MetalTrade.Web.Common.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<MetalTypeDto, MetalTypeViewModel>().ReverseMap();
            CreateMap<ProductDto, ProductViewModel>().ReverseMap();

            CreateMap<AdvertisementPhotoDto, AdvertisementPhotoViewModel>().ReverseMap();

            CreateMap<AdvertisementDto, AdvertisementViewModel>()
                .ForMember(dest => dest.User, opt => opt.Ignore()) 
                .ForMember(dest => dest.Photoes, opt => opt.MapFrom(src => src.Photoes))
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product))
                .ReverseMap()
                .ForMember(dest => dest.User, opt => opt.Ignore()) 
                .ForMember(dest => dest.Photoes, opt => opt.MapFrom(src => src.Photoes))
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product));

            CreateMap<AdvertisementDto, EditViewModel>()
                .ForMember(dest => dest.Photoes, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(dest => dest.Photoes, opt => opt.Ignore())
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
