using System.Security.Claims;
using AutoMapper;
using MetalTrade.Business.Interfaces;
using MetalTrade.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

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
        
        protected AccountController AccountController { get; private set; }

        protected void InitAccountController()
        {
            AccountController = new AccountController(
                UserMock.Object,
                MapperMock.Object,
                ImageUploadMock.Object
            );

            AccountController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity())
                }
            };
        }

        protected void SetAuthenticated()
        {
            var identity = new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.Name, "user1111") },
                "mock"
            );

            AccountController.ControllerContext.HttpContext.User =
                new ClaimsPrincipal(identity);
        }
    }
}
