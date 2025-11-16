using AutoMapper;
using MetalTrade.Business.Dtos;
using MetalTrade.Web.ViewModels.Advertisement;
using MetalTrade.Web.ViewModels.AdvertisementPhoto;
using MetalTrade.Web.ViewModels.Product;

namespace MetalTrade.Web.Common.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AdvertisementDto, AdvertisementViewModel>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Photoes, opt => opt.MapFrom(src => src.Photoes))
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product))
                .ReverseMap();

            CreateMap<AdvertisementPhotoDto, AdvertisementPhotoViewModel>()
                .ReverseMap(); 

            CreateMap<ProductDto, ProductViewModel>()
                .ForMember(dest => dest.MetalType, opt => opt.Ignore()) 
                .ReverseMap();

            CreateMap<AdvertisementDto, EditViewModel>()
                .ForMember(dest => dest.Photoes, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(dest => dest.Photoes, opt => opt.Ignore());

            CreateMap<AdvertisementDto, EditViewModel>()
                .ForMember(dest => dest.Photoes, opt => opt.Ignore()) 
                .ReverseMap()
                .ForMember(dest => dest.Photoes, opt => opt.Ignore());

            CreateMap<AdvertisementDto, DeleteViewModel>()
                .ReverseMap();
        }
    }
}
