using AutoMapper;
using MetalTrade.Business.Dtos;
using MetalTrade.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MetalTrade.Business.Common.Mapping
{
    internal class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<Advertisement, AdvertisementDto>().ReverseMap();
            CreateMap<AdvertisementPhoto, AdvertisementPhotoDto>().ReverseMap();
            CreateMap<MetalType, MetalTypeDto>().ReverseMap();
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
        }
    } 
}
