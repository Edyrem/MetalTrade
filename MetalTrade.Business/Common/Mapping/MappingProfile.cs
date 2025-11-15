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
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<AdvertisementDto, Advertisement>().ReverseMap();
            CreateMap<AdvertisementPhotoDto, AdvertisementPhoto>().ReverseMap();
           // CreateMap<MetalTypeDto, MetalType>().ReverseMap();
            CreateMap<ProductDto, Product>().ReverseMap();
          //  CreateMap<UserDto, User>().ReverseMap();
        }
    } 
}
