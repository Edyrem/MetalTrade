using AutoMapper;
using MetalTrade.Business.Dtos;
using MetalTrade.Business.Interfaces;
using MetalTrade.Web.Controllers;
using MetalTrade.Web.ViewModels.MetalType;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace MetalTrade.Test.ControllersTests;

public class MetalTypeControllerTests
{
    [Fact]
    public async Task IndexReturnsViewWithMetalTypes()
    {
        // Arrange
        var metalDtos = new List<MetalTypeDto>
        {
            new MetalTypeDto { Id = 1, Name = "Steel" },
            new MetalTypeDto { Id = 2, Name = "Copper" }
        };

        var mockMetal = new Mock<IMetalService>();
        var mockMapper = new Mock<IMapper>();

        mockMetal.Setup(s=> s.GetAllAsync()).ReturnsAsync(metalDtos);
        var controller = new MetalTypeController(mockMetal.Object, mockMapper.Object);

        // Act
        var result = await controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<List<MetalTypeViewModel>>(viewResult.Model);

        Assert.Equal(2, model.Count);
        Assert.Equal("Steel", model[0].Name);
        Assert.Equal("Copper", model[1].Name);

        mockMetal.Verify(s => s.GetAllAsync());
    }
    
    
    [Fact]
    public async Task DetailsWhenFoundReturnsView()
    {
        // Arrange
        var dto = new MetalTypeDto { Id = 1, Name = "Steel" };

        var mockMetal = new Mock<IMetalService>();
        var mockMapper = new Mock<IMapper>();

        mockMetal.Setup(s => s.GetAsync(1)).ReturnsAsync(dto);
        
        var controller = new MetalTypeController(mockMetal.Object, mockMapper.Object);

        // Act
        var result = await controller.Details(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<MetalTypeViewModel>(viewResult.Model);

        Assert.Equal(1, model.Id);
        Assert.Equal("Steel", model.Name);
    }
    
    [Fact]
    public async Task DetailsWhenNullRedirectsToIndex()
    {
        // Arrange
        var mockMetal = new Mock<IMetalService>();
        var mockMapper = new Mock<IMapper>();

        mockMetal.Setup(s => s.GetAsync(1)).ReturnsAsync((MetalTypeDto?)null);

        var controller = new MetalTypeController(mockMetal.Object, mockMapper.Object);

        // Act
        var result = await controller.Details(1);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }
    
    [Fact]
    public async Task CreatePostValidModelRedirectsToIndex()
    {
        // Arrange
        var mockMetal = new Mock<IMetalService>();
        var mockMapper = new Mock<IMapper>();
        
        mockMetal.Setup(s => s.CreateAsync(It.IsAny<MetalTypeDto>())).Returns(Task.CompletedTask);

        var controller = new MetalTypeController(mockMetal.Object, mockMapper.Object);

        var model = new CreateMetalViewModel
        {
            Name = "Steel"
        };

        // Act
        var result = await controller.Create(model);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);

        mockMetal.Verify(s => s.CreateAsync(It.IsAny<MetalTypeDto>()));
    }
    
    
    [Fact]
    public async Task CreatePostInvalidModelReturnsView()
    {
        // Arrange
        var mockMetal = new Mock<IMetalService>();
        var mockMapper = new Mock<IMapper>();
        var controller = new MetalTypeController(mockMetal.Object, mockMapper.Object);

        controller.ModelState.AddModelError("Name", "Required");

        var model = new CreateMetalViewModel();
        
        // Act
        var result = await controller.Create(model);

        // Assert
        var view = Assert.IsType<ViewResult>(result);
        Assert.Equal(model, view.Model);

        mockMetal.Verify(s => s.CreateAsync(It.IsAny<MetalTypeDto>()), Times.Never);
    }
    
    [Fact]
    public void CreateGetReturnsView()
    {
        // Arrange
        var mockMetal = new Mock<IMetalService>();
        var mockMapper = new Mock<IMapper>();
        var controller = new MetalTypeController(mockMetal.Object, mockMapper.Object);

        // Act
        var result = controller.Create();

        // Assert
        Assert.IsType<ViewResult>(result);
    }
    
    [Fact]
    public async Task EditGetWhenFoundReturnsView()
    {
        // Arrange
        var dto = new MetalTypeDto { Id = 1, Name = "Steel" };

        var mockMetal = new Mock<IMetalService>();
        var mockMapper = new Mock<IMapper>();
        mockMetal.Setup(s => s.GetAsync(1)).ReturnsAsync(dto);

        var controller = new MetalTypeController(mockMetal.Object, mockMapper.Object);

        // Act
        var result = await controller.Edit(1);

        // Assert
        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<EditMetalViewModel>(view.Model);

        Assert.Equal(1, model.Id);
    }

    [Fact]
    public async Task EditGetWhenNullRedirectsToIndex()
    {
        // Arrange
        var mockMetal = new Mock<IMetalService>();
        var mockMapper = new Mock<IMapper>();
        mockMetal.Setup(s => s.GetAsync(1)).ReturnsAsync((MetalTypeDto?)null);

        var controller = new MetalTypeController(mockMetal.Object, mockMapper.Object);

        // Act
        var result = await controller.Edit(1);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }
    
    [Fact]
    public async Task EditPostValidModelRedirectsToIndex()
    {
        // Arrange
        var mockMetal = new Mock<IMetalService>();
        var mockMapper = new Mock<IMapper>();
        mockMetal.Setup(s => s.UpdateAsync(It.IsAny<MetalTypeDto>())).Returns(Task.CompletedTask);
        var controller = new MetalTypeController(mockMetal.Object, mockMapper.Object);

        var model = new EditMetalViewModel
        {
            Id = 1,
            Name = "Steel"
        };

        // Act
        var result = await controller.Edit(model);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        mockMetal.Verify(s => s.UpdateAsync(It.IsAny<MetalTypeDto>()));
    }

    [Fact]
    public async Task EditPostInvalidModelReturnsView()
    {
        // Arrange
        var mockMetal = new Mock<IMetalService>();
        var mockMapper = new Mock<IMapper>();
        var controller = new MetalTypeController(mockMetal.Object, mockMapper.Object);

        controller.ModelState.AddModelError("Name", "Required");

        var model = new EditMetalViewModel();

        // Act
        var result = await controller.Edit(model);

        // Assert
        var view = Assert.IsType<ViewResult>(result);
        Assert.Equal(model, view.Model);

        mockMetal.Verify(s => s.UpdateAsync(It.IsAny<MetalTypeDto>()), Times.Never);
    }

    
    [Fact]
    public async Task DeleteGetWhenFoundReturnsView()
    {
        // Arrange
        var dto = new MetalTypeDto { Id = 1, Name = "Steel" };

        var mockMetal = new Mock<IMetalService>();
        var mockMapper = new Mock<IMapper>();
        mockMetal.Setup(s => s.GetAsync(1)).ReturnsAsync(dto);
        var controller = new MetalTypeController(mockMetal.Object, mockMapper.Object);

        // Act
        var result = await controller.Delete(1);

        // Assert
        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<DeleteMetalViewModel>(view.Model);
        Assert.Equal(1, model.Id);
    }

    [Fact]
    public async Task DeleteGetWhenNullRedirectsToIndex()
    {
        // Arrange
        var mockMetal = new Mock<IMetalService>();
        var mockMapper = new Mock<IMapper>();
        mockMetal.Setup(s => s.GetAsync(1)).ReturnsAsync((MetalTypeDto?)null);
        var controller = new MetalTypeController(mockMetal.Object, mockMapper.Object);

        // Act
        var result = await controller.Delete(1);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }
    
    [Fact]
    public async Task DeletePostRedirectsToIndex()
    {
        // Arrange
        var mockMetal = new Mock<IMetalService>();
        var mockMapper = new Mock<IMapper>();
        mockMetal.Setup(s => s.DeleteAsync(1)).Returns(Task.CompletedTask);
        var controller = new MetalTypeController(mockMetal.Object, mockMapper.Object);

        var model = new DeleteMetalViewModel { Id = 1 };

        // Act
        var result = await controller.Delete(model);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);

        mockMetal.Verify(s => s.DeleteAsync(1));
    }
}