using AutoMapper;
using Castle.Core.Logging;
using MetalTrade.Business.Interfaces;
using MetalTrade.Web.AdminPanel.Controllers;
using MetalTrade.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;

namespace MetalTrade.Test.Helpers
{
    public abstract class ControllerTestBase
    {
        protected Mock<IAdvertisementService> AdvertisementMock = new();
        protected Mock<IProductService> ProductMock = new();        
        protected Mock<IMetalService> MetalMock = new();
        protected Mock<IMapper> MapperMock = new();
        protected Mock<IImageUploadService> ImageUploadMock = new();
        protected Mock<IUserService> UserMock = new();
        protected Mock<ILogger<AdvertisementController>> LoggerMock = new();

        protected Mock<ICommercialService> CommercialMock = new();
        protected Mock<IAdvertisementImportService> AdvertisementImportMock  = new();

        

        protected ProductController ProductController
            => new(ProductMock.Object, MetalMock.Object, MapperMock.Object);

        protected MetalTypeController MetalTypeController
            => new(MetalMock.Object, MapperMock.Object);
        
        protected AccountController AccountController 
        { 
            get
            {
                var controller = new AccountController(
                    UserMock.Object,
                    MapperMock.Object,
                    ImageUploadMock.Object
                );
                controller.ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity())
                    }
                };
                return controller;
            }            
        }
        
        protected AccountController AuthenticatedAccountController 
        { 
            get
            {
                var controller = new AccountController(
                    UserMock.Object,
                    MapperMock.Object,
                    ImageUploadMock.Object
                );
                var identity = new ClaimsIdentity(
                    new[] { new Claim(ClaimTypes.Name, "user123") },
                    "mock"
                );
                controller.ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(identity)
                    }
                };
                return controller;
            }
        }
      
        protected UserController CreateControllerWithUser(params Claim[] claims)
        {
            var controller = new UserController(UserMock.Object, MapperMock.Object);

            var identity = new ClaimsIdentity(claims, "mock");
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(identity)
                }
            };

            return controller;
        }
        
        protected AdvertisementController AdvertisementController
        {
            get
            {
                var controller = new AdvertisementController(
                    AdvertisementMock.Object,
                    UserMock.Object,
                    ProductMock.Object,
                    MetalMock.Object,
                    MapperMock.Object,
                    LoggerMock.Object,
                    AdvertisementImportMock.Object
                );

                var identity = new ClaimsIdentity(
                    new[] { new Claim(ClaimTypes.NameIdentifier, "5") },
                    "mock"
                );

                controller.ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(identity)
                    }
                };

                return controller;
            }
        }

        
    }
}
