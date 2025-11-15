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
                .ForMember(dest => dest.Photoes, opt => opt.MapFrom(src => src.Photoes))
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product))
                .ReverseMap();

            CreateMap<AdvertisementPhotoDto, AdvertisementPhotoViewModel>()
                .ReverseMap(); 

            CreateMap<ProductDto, ProductViewModel>()
                .ReverseMap();

            CreateMap<CreateViewModel, AdvertisementDto>()
                .ForMember(dest => dest.Photoes, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(dest => dest.Photoes, opt => opt.Ignore()); 
            

            CreateMap<EditViewModel, AdvertisementDto>()
                .ForMember(dest => dest.Photoes, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(dest => dest.Photoes, opt => opt.Ignore());

            CreateMap<AdvertisementDto, DeleteViewModel>()
                .ReverseMap();
        }
    }
}
