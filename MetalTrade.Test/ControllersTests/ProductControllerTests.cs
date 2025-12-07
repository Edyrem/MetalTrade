using MetalTrade.Business.Dtos;
using MetalTrade.Business.Interfaces;
using MetalTrade.Web.Controllers;
using MetalTrade.Web.ViewModels.MetalType;
using MetalTrade.Web.ViewModels.Product;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace MetalTrade.Test.ControllersTests;

public class ProductControllerTests
{
    
    [Fact]
    public async Task CreateGetReturnsView()
    {
        // Arrange
        var metalDtos = new List<MetalTypeDto>
        {
            new () { Id = 1, Name = "сталь" },
            new() { Id = 2, Name = "железо" }
        };
        var mockProduct = new Mock<IProductService>();
        var mockMetal = new Mock<IMetalService>();
        
        mockMetal.Setup(s => s.GetAllAsync()).ReturnsAsync(metalDtos);
        var controller = new ProductController(mockProduct.Object, mockMetal.Object);

        // Act
        var result = await controller.Create();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<CreateProductViewModel>(viewResult.Model);
        
        Assert.Equal(2, model.MetalTypes.Count);
        Assert.Equal("сталь", model.MetalTypes[0].Text);
        Assert.Equal("железо", model.MetalTypes[1].Text);
        Assert.NotNull(model.MetalTypes);
        
        mockMetal.Verify(m => m.GetAllAsync());
    }
    
    
    [Fact]
    public async Task CreatePostValidModelRedirectsToIndex()
    {
        // Arrange
        var product = new CreateProductViewModel { Name = "труба", MetalTypeId = 1 };
        
        var mockProduct = new Mock<IProductService>();
        var mockMetal = new Mock<IMetalService>();
        
        mockProduct.Setup(s => s.CreateAsync(It.IsAny<ProductDto>())).Returns(Task.CompletedTask);
        var controller = new ProductController(mockProduct.Object, mockMetal.Object);

        // Act
        var result = await controller.Create(product);
        
        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);

        mockProduct.Verify(s => s.CreateAsync(It.IsAny<ProductDto>()));
    }
    
    [Fact]
    public async Task CreatePostInvalidModelReturnsView()
    {
        // Arrange
        
        var mockProduct = new Mock<IProductService>();
        var mockMetal = new Mock<IMetalService>();
        
        var controller = new ProductController(mockProduct.Object, mockMetal.Object);
        controller.ModelState.AddModelError("Name", "Required");

        var model = new CreateProductViewModel();
        
        // Act
        var result = await controller.Create(model);
        
        // Assert
        var view = Assert.IsType<ViewResult>(result);
        Assert.IsType<CreateProductViewModel>(view.Model);
        Assert.Equal(model, view.Model);
        
        mockProduct.Verify(s => s.CreateAsync(It.IsAny<ProductDto>()), Times.Never);
    }
    

    [Fact]
    public async Task IndexReturnsViewWithProducts()
    {
        // Arrange
        
        var metalDtos = new List<MetalTypeDto>
        {
            new MetalTypeDto { Id = 1, Name = "сталь" },
            new MetalTypeDto { Id = 2, Name = "железо" }
        };
        
        var productDtos = new List<ProductDto>
        {
            new ProductDto { Id = 1, Name = "труба", MetalTypeId = metalDtos[0].Id, MetalType = metalDtos[0]},
            new ProductDto { Id = 2, Name = "арматура" , MetalTypeId = metalDtos[1].Id, MetalType = metalDtos[1]}
        };

        var mockMetal = new Mock<IMetalService>();
        var mockProduct = new Mock<IProductService>();
        mockMetal.Setup(s => s.GetAllAsync()).ReturnsAsync(metalDtos);
        mockProduct.Setup(p=> p.GetAllAsync()).ReturnsAsync(productDtos);
        
        var controller = new ProductController(mockProduct.Object, mockMetal.Object);

        // Act
        var result = await controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<List<ProductViewModel>>(viewResult.Model);

        Assert.Equal("сталь", model[0].MetalType.Name);
        Assert.Equal("железо", model[1].MetalType.Name);
        Assert.Equal(2, model.Count);
        Assert.Equal("труба", model[0].Name);
        Assert.Equal("арматура", model[1].Name);

        mockProduct.Verify(p => p.GetAllAsync());
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

        var mockMetal = new Mock<IMetalService>();
        var mockProduct = new Mock<IProductService>();
        mockProduct.Setup(p=> p.GetAsync(1)).ReturnsAsync(product);
        
        var controller = new ProductController(mockProduct.Object, mockMetal.Object);

        // Act
        var result = await controller.Details(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ProductViewModel>(viewResult.Model);

        Assert.Equal("труба", model.Name);
        Assert.Equal(1, model.MetalTypeId);
        Assert.Equal(1, model.Id);

        mockProduct.Verify(p => p.GetAsync(1));
    }

    [Fact]
    public async Task DetailsWhenNullRedirectsToIndex()
    {
        // Arrange
        
        var mockMetal = new Mock<IMetalService>();
        var mockProduct = new Mock<IProductService>();
        mockProduct.Setup(p=> p.GetAsync(1)).ReturnsAsync((ProductDto?)null);
        
        var controller = new ProductController(mockProduct.Object, mockMetal.Object);

        // Act
        var result = await controller.Details(1);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        mockProduct.Verify(p => p.GetAsync(1));
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
        
        var mockMetal = new Mock<IMetalService>();
        var mockProduct = new Mock<IProductService>();
        mockProduct.Setup(p => p.GetAsync(1)).ReturnsAsync(product);
        mockMetal.Setup(m => m.GetAllAsync()).ReturnsAsync(metalTypes);
        
        var controller = new ProductController(mockProduct.Object, mockMetal.Object);

        // Act
        var result = await controller.Edit(1);
        
        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<EditProductViewModel>(viewResult.Model);
        
        Assert.Equal("труба", model.Name);
        Assert.Equal(1, model.MetalTypeId);
        Assert.Equal(1, model.Id);
        Assert.Equal(2, model.MetalTypes.Count);
        Assert.Equal("сталь", model.MetalTypes[0].Text);
        Assert.Equal("железо", model.MetalTypes[1].Text);
        
        mockProduct.Verify(p => p.GetAsync(1));
        mockMetal.Verify(p=>p.GetAllAsync());
    }
    
    [Fact]
    public async Task EditGetWhenNullRedirectsToIndex()
    {
        // Arrange
        
        var mockMetal = new Mock<IMetalService>();
        var mockProduct = new Mock<IProductService>();
        mockProduct.Setup(p=> p.GetAsync(1)).ReturnsAsync((ProductDto?)null);
        
        var controller = new ProductController(mockProduct.Object, mockMetal.Object);

        // Act
        var result = await controller.Edit(1);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        mockProduct.Verify(p => p.GetAsync(1));
    }
    
    
    [Fact]
    public async Task EditPostValidModelRedirectsToIndex()
    {
        // Arrange
        
        var mockProduct = new Mock<IProductService>();
        var mockMetal = new Mock<IMetalService>();
        
        mockProduct.Setup(s => s.UpdateAsync(It.IsAny<ProductDto>())).Returns(Task.CompletedTask);
        var controller = new ProductController(mockProduct.Object, mockMetal.Object);

        var model = new EditProductViewModel() 
        {
            Id = 1,
            Name = "труба",
            MetalTypeId = 1
        };
        
        // Act
        var result = await controller.Edit(model);
        
        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);

        mockProduct.Verify(s => s.UpdateAsync(It.IsAny<ProductDto>()));
    }
    
    [Fact]
    public async Task EditPostInvalidModelReturnsView()
    {
        // Arrange
        
        var mockProduct = new Mock<IProductService>();
        var mockMetal = new Mock<IMetalService>();
        
        var controller = new ProductController(mockProduct.Object, mockMetal.Object);
        controller.ModelState.AddModelError("Name", "Required");

        var model = new EditProductViewModel();
        
        // Act
        var result = await controller.Edit(model);
        
        // Assert
        var view = Assert.IsType<ViewResult>(result);
        Assert.Equal(model, view.Model);
        
        mockProduct.Verify(s => s.UpdateAsync(It.IsAny<ProductDto>()), Times.Never);
    }


    [Fact]
    public async Task DeleteGetWhenNullRedirectsToIndex()
    {
        // Arrange
        
        var mockMetal = new Mock<IMetalService>();
        var mockProduct = new Mock<IProductService>();
        mockProduct.Setup(p=> p.GetAsync(1)).ReturnsAsync((ProductDto?)null);
        
        var controller = new ProductController(mockProduct.Object, mockMetal.Object);

        // Act
        var result = await controller.Delete(1);

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
        
        var mockMetal = new Mock<IMetalService>();
        var mockProduct = new Mock<IProductService>();
        mockProduct.Setup(p=> p.GetAsync(1)).ReturnsAsync(product);
        
        var controller = new ProductController(mockProduct.Object, mockMetal.Object);
        
        // Act
        var result = await controller.Delete(1);

        // Assert
        
        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<DeleteProductViewModel>(view.Model);
        Assert.Equal(1, model.Id);
    }
    
    
    [Fact]
    public async Task DeletePostRedirectsToIndex()
    {
        // Arrange
        
        var mockMetal = new Mock<IMetalService>();
        var mockProduct = new Mock<IProductService>();
        mockProduct.Setup(s => s.DeleteAsync(1)).Returns(Task.CompletedTask);
        var controller = new ProductController(mockProduct.Object, mockMetal.Object);

        var model = new DeleteProductViewModel() { Id = 1 };

        // Act
        var result = await controller.Delete(model);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);

        mockProduct.Verify(s => s.DeleteAsync(1));
    }

}