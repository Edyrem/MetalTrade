using AutoMapper;
using MetalTrade.Business.Interfaces;
using MetalTrade.Web.AdminPanel.Controllers;
using MetalTrade.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace MetalTrade.Test.Helpers
{
    public abstract class ControllerTestBase
    {
        protected Mock<IProductService> ProductMock = new();        
        protected Mock<IMetalService> MetalMock = new();
        protected Mock<IMapper> MapperMock = new();
        protected Mock<IImageUploadService> ImageUploadMock = new();
        protected Mock<IUserService> UserMock = new();

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
    }
}
