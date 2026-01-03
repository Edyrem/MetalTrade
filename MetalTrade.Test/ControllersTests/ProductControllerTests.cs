using AutoMapper;
using MetalTrade.Business.Dtos;
using MetalTrade.Business.Interfaces;
using MetalTrade.Test.Helpers;
using MetalTrade.Web.Controllers;
using MetalTrade.Web.ViewModels.MetalType;
using MetalTrade.Web.ViewModels.Product;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using Xunit;

namespace MetalTrade.Test.ControllersTests;

public class ProductControllerTests: ControllerTestBase
{

    [Fact]
    public async Task IndexReturnsViewWithMappedProducts()
    {
        // Arrange
        var productDtos = new List<ProductDto>
    {
        new() { Id = 1, Name = "Сталь", MetalTypeId = 1 },
        new() { Id = 2, Name = "Железо", MetalTypeId = 2 }
    };

        var viewModels = new List<ProductViewModel>
    {
        new() { Id = 1, Name = "Сталь", MetalTypeId = 1 },
        new() { Id = 2, Name = "Железо", MetalTypeId = 2 }
    };

        ProductMock
            .Setup(p => p.GetFilteredAsync(It.IsAny<ProductFilterDto>()))
            .ReturnsAsync(productDtos);

        MapperMock
            .Setup(m => m.Map<List<ProductViewModel>>(productDtos))
            .Returns(viewModels);

        var controller = ProductController;

        // Эмулируем HttpContext и Request.Query
        var query = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
        {
            ["name"] = "",
            ["metalTypeId"] = "",
            ["sort"] = "",
            ["page"] = "1"
        });

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                Request =
            {
                Query = query
            }
            }
        };

        // Act
        var result = await controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<ProductViewModel>>(viewResult.Model);

        Assert.Equal(2, model.Count);
        Assert.Equal("Сталь", model[0].Name);

        ProductMock.Verify(p => p.GetFilteredAsync(It.IsAny<ProductFilterDto>()), Times.Once);
        MapperMock.Verify(m => m.Map<List<ProductViewModel>>(productDtos), Times.Once);
    }

    [Fact]
    public async Task CreateGetReturnsViewWithMetalTypesInViewData()
    {
        // Arrange
        var metalTypes = new List<MetalTypeDto>
            {
                new() { Id = 1, Name = "Сталь" },
                new() { Id = 2, Name = "Железо" }
            };

        MetalMock
            .Setup(m => m.GetAllAsync())
            .ReturnsAsync(metalTypes);

        // Act
        var result = await ProductController.Create();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);

        Assert.True(viewResult.ViewData.ContainsKey("MetalTypes"));

        var selectList = Assert.IsType<SelectList>(viewResult.ViewData["MetalTypes"]);
        Assert.Equal(2, selectList.Count());

        MetalMock.Verify(m => m.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task CreatePostInvalidModelReturnsViewWithSameModel()
    {
        // Arrange
        var controller = new ProductController(ProductMock.Object, MetalMock.Object, MapperMock.Object);
        controller.ModelState.AddModelError("Name", "Required");
        
        var model = new CreateProductViewModel(); 
        
        // Act
        var result = await controller.Create(model);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var returnedModel = Assert.IsType<CreateProductViewModel>(viewResult.Model);

        Assert.Equal(model, returnedModel);

        ProductMock.Verify(p => p.CreateAsync(It.IsAny<ProductDto>()), Times.Never);
        MapperMock.Verify(m => m.Map<ProductDto>(It.IsAny<CreateProductViewModel>()), Times.Never);
    }

    [Fact]
    public async Task CreatePostValidModel_RedirectsToIndex()
    {
        // Arrange
        var model = new CreateProductViewModel
        {
            Name = "сталь",
            MetalTypeId = 1
        };

        var productDto = new ProductDto
        {
            Name = "сталь",
            MetalTypeId = 1
        };

        MapperMock
            .Setup(m => m.Map<ProductDto>(model))
            .Returns(productDto);
        
        ProductMock
            .Setup(s => s.CreateAsync(productDto))
            .Returns(Task.CompletedTask);

        // Act
        var result = await ProductController.Create(model);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);

        MapperMock.Verify(m => m.Map<ProductDto>(model), Times.Once);
        ProductMock.Verify(p => p.CreateAsync(productDto), Times.Once);
    }

    [Fact]
    public async Task CreateGetReturnViewTest()
    {
        // Arrange
        MetalMock.Setup(m => m.GetAllAsync()).ReturnsAsync(new List<MetalTypeDto>());

        // Act
        var result = await ProductController.Create();

        // Assert
        Assert.IsType<ViewResult>(result);    
    }
    
    [Fact]
    public async Task CreateGetReturnsView()
    {
        // Arrange
        var metalDtos = new List<MetalTypeDto>
        {
            new () { Id = 1, Name = "сталь" },
            new() { Id = 2, Name = "железо" }
        };
        
        MetalMock.Setup(s => s.GetAllAsync()).ReturnsAsync(metalDtos);
        
        // Act
        var result = await ProductController.Create();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);    
        
        Assert.NotNull(viewResult.ViewData["MetalTypes"]);
        var selectList = Assert.IsType<SelectList>(viewResult.ViewData["MetalTypes"]);
        Assert.Equal(2, selectList.Count());
        
        MetalMock.Verify(m => m.GetAllAsync());
    }
    
    
    [Fact]
    public async Task CreatePostValidModelRedirectsToIndex()
    {
        // Arrange
        var product = new CreateProductViewModel { Name = "труба", MetalTypeId = 1 };

        ProductMock.Setup(s => s.CreateAsync(It.IsAny<ProductDto>())).Returns(Task.CompletedTask);

        // Act
        var result = await ProductController.Create(product);
        
        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);

        ProductMock.Verify(s => s.CreateAsync(It.IsAny<ProductDto>()));
    }
    
    [Fact]
    public async Task CreatePostInvalidModelReturnsView()
    {
        // Arrange
        var controller = new ProductController(ProductMock.Object, MetalMock.Object, MapperMock.Object);

        controller.ModelState.AddModelError("Name", "Required");

        var model = new CreateProductViewModel();
        
        // Act
        var result = await controller.Create(model);
        
        // Assert
        var view = Assert.IsType<ViewResult>(result);
        Assert.IsType<CreateProductViewModel>(view.Model);
        Assert.Equal(model, view.Model);
        
        ProductMock.Verify(s => s.CreateAsync(It.IsAny<ProductDto>()), Times.Never);
    }


    [Fact]
    public async Task IndexReturnsViewWithProducts()
    {
        // Arrange
        var productDtos = new List<ProductDto>
    {
        new ProductDto { Id = 1, Name = "труба", MetalTypeId = 1, MetalType = new MetalTypeDto  { Id = 1, Name = "сталь" }},
        new ProductDto { Id = 2, Name = "арматура" , MetalTypeId = 2, MetalType = new MetalTypeDto  { Id = 2, Name = "железо" }}
    };

        var productViewModels = new List<ProductViewModel>
    {
        new ProductViewModel { Id = 1, Name = "труба", MetalTypeId = 1, MetalType = new MetalTypeViewModel { Id = 1, Name = "сталь" } },
        new ProductViewModel { Id = 2, Name = "арматура", MetalTypeId = 2, MetalType = new MetalTypeViewModel { Id = 2, Name = "железо" } }
    };

        // Мокируем GetFilteredAsync, так как Index использует фильтр
        ProductMock
            .Setup(p => p.GetFilteredAsync(It.IsAny<ProductFilterDto>()))
            .ReturnsAsync(productDtos);

        MapperMock
            .Setup(m => m.Map<List<ProductViewModel>>(productDtos))
            .Returns(productViewModels);

        var controller = ProductController;

        // Эмулируем Request.Query, чтобы Request != null
        var query = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
        {
            ["name"] = "",
            ["metalTypeId"] = "",
            ["sort"] = "",
            ["page"] = "1"
        });

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                Request =
            {
                Query = query
            }
            }
        };

        // Act
        var result = await controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<List<ProductViewModel>>(viewResult.Model);

        Assert.Equal(2, model.Count);
        Assert.Equal("труба", model[0].Name);
        Assert.Equal("арматура", model[1].Name);

        ProductMock.Verify(p => p.GetFilteredAsync(It.IsAny<ProductFilterDto>()), Times.Once);
        MapperMock.Verify(m => m.Map<List<ProductViewModel>>(productDtos), Times.Once);
    }

    [Fact]
    public async Task DetailsWhenFoundReturnsView()
    {
        // Arrange
        
        var product = new ProductDto 
        {
            Id = 1,
            Name = "труба",
            MetalTypeId = 1,
            MetalType = new MetalTypeDto
            {
                Id = 1,
                Name = "сталь"
            }
        };

        ProductMock.Setup(p=> p.GetAsync(1)).ReturnsAsync(product);        
        MapperMock.Setup(m => m.Map<ProductViewModel>(product))
            .Returns(new ProductViewModel { Id = 1, Name = "труба", MetalTypeId = 1 });
        
        // Act
        var result = await ProductController.Details(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ProductViewModel>(viewResult.Model);

        Assert.Equal("труба", model.Name);
        Assert.Equal(1, model.MetalTypeId);
        Assert.Equal(1, model.Id);

        ProductMock.Verify(p => p.GetAsync(1));
        MapperMock.Verify(m => m.Map<ProductViewModel>(product));

    }

    [Fact]
    public async Task DetailsWhenNullRedirectsToIndex()
    {
        // Arrange
        ProductMock.Setup(p=> p.GetAsync(1)).ReturnsAsync((ProductDto?)null);

        // Act
        var result = await ProductController.Details(1);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        ProductMock.Verify(p => p.GetAsync(1));
    }
    
    
    [Fact]
    public async Task EditGetWhenFoundReturnsView()
    {
        var product = new ProductDto 
        {
            Id = 1,
            Name = "труба",
            MetalTypeId = 1,
            MetalType = new MetalTypeDto
            {
                Id = 1,
                Name = "сталь"
            }
        };
        
        var metalTypes = new List<MetalTypeDto>
        {
            new() { Id = 1, Name = "сталь" },
            new () { Id = 2, Name = "железо" }
        };
        
        // Arrange
        ProductMock.Setup(p => p.GetAsync(1)).ReturnsAsync(product);
        MetalMock.Setup(m => m.GetAllAsync()).ReturnsAsync(metalTypes);
        MapperMock.Setup(m => m.Map<EditProductViewModel>(product))
            .Returns(new EditProductViewModel { Id = 1, Name = "труба", MetalTypeId = 1 });

        // Act
        var result = await ProductController.Edit(1);
        
        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<EditProductViewModel>(viewResult.Model);
        
        Assert.Equal("труба", model.Name);
        Assert.Equal(1, model.MetalTypeId);
        Assert.Equal(1, model.Id);

        ProductMock.Verify(p => p.GetAsync(1));
        MetalMock.Verify(p=>p.GetAllAsync());
    }
    
    [Fact]
    public async Task EditGetWhenNullRedirectsToIndex()
    {
        // Arrange
        ProductMock.Setup(p=> p.GetAsync(1)).ReturnsAsync((ProductDto?)null);
        
        // Act
        var result = await ProductController.Edit(1);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        ProductMock.Verify(p => p.GetAsync(1));
    }
    
    
    [Fact]
    public async Task EditPostValidModelRedirectsToIndex()
    {
        // Arrange
        ProductMock.Setup(s => s.UpdateAsync(It.IsAny<ProductDto>())).Returns(Task.CompletedTask);

        var model = new EditProductViewModel() 
        {
            Id = 1,
            Name = "труба",
            MetalTypeId = 1
        };
        
        // Act
        var result = await ProductController.Edit(model);
        
        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);

        ProductMock.Verify(s => s.UpdateAsync(It.IsAny<ProductDto>()));
    }
    
    [Fact]
    public async Task EditPostInvalidModelReturnsView()
    {
        // Arrange
        var controller = new ProductController(ProductMock.Object, MetalMock.Object, MapperMock.Object);

        controller.ModelState.AddModelError("Name", "Required");

        var model = new EditProductViewModel();
        
        // Act
        var result = await controller.Edit(model);
        
        // Assert
        var view = Assert.IsType<ViewResult>(result);
        Assert.Equal(model, view.Model);

        ProductMock.Verify(s => s.UpdateAsync(It.IsAny<ProductDto>()), Times.Never);
    }


    [Fact]
    public async Task DeleteGetWhenNullRedirectsToIndex()
    {
        // Arrange
        ProductMock.Setup(p=> p.GetAsync(1)).ReturnsAsync((ProductDto?)null);

        // Act
        var result = await ProductController.Delete(1);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }
    
    [Fact]
    public async Task DeleteGetWhenFoundReturnsView()
    {
        // Arrange
        
        var product = new ProductDto 
        {
            Id = 1,
            Name = "труба",
            MetalTypeId = 1,
            MetalType = new MetalTypeDto
            {
                Id = 1,
                Name = "сталь"
            }
        };
        
        ProductMock.Setup(p=> p.GetAsync(1)).ReturnsAsync(product);        
        
        // Act
        var result = await ProductController.Delete(1);

        // Assert
        
        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<DeleteProductViewModel>(view.Model);
        Assert.Equal(1, model.Id);
    }
    
    
    [Fact]
    public async Task DeletePostRedirectsToIndex()
    {
        // Arrange        
        ProductMock.Setup(s => s.DeleteAsync(1)).Returns(Task.CompletedTask);

        var model = new DeleteProductViewModel() { Id = 1 };

        // Act
        var result = await ProductController.Delete(model);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);

        ProductMock.Verify(s => s.DeleteAsync(1));
    }

}