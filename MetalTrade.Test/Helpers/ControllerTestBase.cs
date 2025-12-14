using AutoMapper;
using MetalTrade.Business.Interfaces;
using MetalTrade.Web.Controllers;
using Moq;

namespace MetalTrade.Test.Helpers
{
    public abstract class ControllerTestBase
    {
        protected Mock<IProductService> ProductMock = new();        
        protected Mock<IMetalService> MetalMock = new();
        protected Mock<IMapper> MapperMock = new();

        protected ProductController ProductController
            => new(ProductMock.Object, MetalMock.Object, MapperMock.Object);

        protected MetalTypeController MetalTypeController
            => new(MetalMock.Object, MapperMock.Object);
    }
}
