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

            // User - маппим все поля КРОМЕ Photo
            CreateMap<UserDto, UserViewModel>()
                .ForMember(dest => dest.Photo, opt => opt.Ignore()) // Игнорируем только Photo
                .ReverseMap()
                .ForMember(dest => dest.Photo, opt => opt.Ignore()); // Игнорируем только Photo

            CreateMap<AdvertisementPhotoDto, AdvertisementPhotoViewModel>().ReverseMap();

            // Advertisement - полный маппинг
            CreateMap<AdvertisementDto, AdvertisementViewModel>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.Photoes, opt => opt.MapFrom(src => src.Photoes))
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product))
                .ReverseMap()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.Photoes, opt => opt.MapFrom(src => src.Photoes))
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product));

            // Edit - картинки игнорируем только для формы редактирования
            CreateMap<AdvertisementDto, EditViewModel>()
                .ReverseMap()
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());

            CreateMap<AdvertisementDto, DeleteViewModel>().ReverseMap();

            CreateMap<CreateViewModel, AdvertisementDto>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.Photoes, opt => opt.Ignore()) // В создании фото обрабатываем отдельно
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());
        }
    }
}
