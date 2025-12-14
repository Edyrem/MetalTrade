using AutoMapper;
using MetalTrade.Business.Dtos;
using MetalTrade.Business.Interfaces;
using MetalTrade.Test.Helpers;
using MetalTrade.Web.Controllers;
using MetalTrade.Web.ViewModels.MetalType;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace MetalTrade.Test.ControllersTests;

public class MetalTypeControllerTests: ControllerTestBase
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

        MetalMock.Setup(s=> s.GetAllAsync()).ReturnsAsync(metalDtos);

        // Act
        var result = await MetalTypeController.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<List<MetalTypeViewModel>>(viewResult.Model);

        Assert.Equal(2, model.Count);
        Assert.Equal("Steel", model[0].Name);
        Assert.Equal("Copper", model[1].Name);

        MetalMock.Verify(s => s.GetAllAsync());
    }
    
    
    [Fact]
    public async Task DetailsWhenFoundReturnsView()
    {
        // Arrange
        var dto = new MetalTypeDto { Id = 1, Name = "Steel" };

        MetalMock.Setup(s => s.GetAsync(1)).ReturnsAsync(dto);        

        // Act
        var result = await MetalTypeController.Details(1);

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
        MetalMock.Setup(s => s.GetAsync(1)).ReturnsAsync((MetalTypeDto?)null);

        // Act
        var result = await MetalTypeController.Details(1);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }
    
    [Fact]
    public async Task CreatePostValidModelRedirectsToIndex()
    {
        // Arrange
        MetalMock.Setup(s => s.CreateAsync(It.IsAny<MetalTypeDto>())).Returns(Task.CompletedTask);

        var model = new CreateMetalViewModel
        {
            Name = "Steel"
        };

        // Act
        var result = await MetalTypeController.Create(model);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);

        MetalMock.Verify(s => s.CreateAsync(It.IsAny<MetalTypeDto>()));
    }
    
    
    [Fact]
    public async Task CreatePostInvalidModelReturnsView()
    {
        // Arrange
        MetalTypeController.ModelState.AddModelError("Name", "Required");

        var model = new CreateMetalViewModel();
        
        // Act
        var result = await MetalTypeController.Create(model);

        // Assert
        var view = Assert.IsType<ViewResult>(result);
        Assert.Equal(model, view.Model);

        MetalMock.Verify(s => s.CreateAsync(It.IsAny<MetalTypeDto>()), Times.Never);
    }
    
    [Fact]
    public void CreateGetReturnsView()
    {
        // Arrange

        // Act
        var result = MetalTypeController.Create();

        // Assert
        Assert.IsType<ViewResult>(result);
    }
    
    [Fact]
    public async Task EditGetWhenFoundReturnsView()
    {
        // Arrange
        var dto = new MetalTypeDto { Id = 1, Name = "Steel" };

        MetalMock.Setup(s => s.GetAsync(1)).ReturnsAsync(dto);

        // Act
        var result = await MetalTypeController.Edit(1);

        // Assert
        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<EditMetalViewModel>(view.Model);

        Assert.Equal(1, model.Id);
    }

    [Fact]
    public async Task EditGetWhenNullRedirectsToIndex()
    {
        // Arrange
        MetalMock.Setup(s => s.GetAsync(1)).ReturnsAsync((MetalTypeDto?)null);

        // Act
        var result = await MetalTypeController.Edit(1);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }
    
    [Fact]
    public async Task EditPostValidModelRedirectsToIndex()
    {
        // Arrange
        MetalMock.Setup(s => s.UpdateAsync(It.IsAny<MetalTypeDto>())).Returns(Task.CompletedTask);

        var model = new EditMetalViewModel
        {
            Id = 1,
            Name = "Steel"
        };

        // Act
        var result = await MetalTypeController.Edit(model);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        MetalMock.Verify(s => s.UpdateAsync(It.IsAny<MetalTypeDto>()));
    }

    [Fact]
    public async Task EditPostInvalidModelReturnsView()
    {
        // Arrange
        MetalTypeController.ModelState.AddModelError("Name", "Required");

        var model = new EditMetalViewModel();

        // Act
        var result = await MetalTypeController.Edit(model);

        // Assert
        var view = Assert.IsType<ViewResult>(result);
        Assert.Equal(model, view.Model);

        MetalMock.Verify(s => s.UpdateAsync(It.IsAny<MetalTypeDto>()), Times.Never);
    }

    
    [Fact]
    public async Task DeleteGetWhenFoundReturnsView()
    {
        // Arrange
        var dto = new MetalTypeDto { Id = 1, Name = "Steel" };

        MetalMock.Setup(s => s.GetAsync(1)).ReturnsAsync(dto);

        // Act
        var result = await MetalTypeController.Delete(1);

        // Assert
        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<DeleteMetalViewModel>(view.Model);
        Assert.Equal(1, model.Id);
    }

    [Fact]
    public async Task DeleteGetWhenNullRedirectsToIndex()
    {
        // Arrange
        MetalMock.Setup(s => s.GetAsync(1)).ReturnsAsync((MetalTypeDto?)null);

        // Act
        var result = await MetalTypeController.Delete(1);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }
    
    [Fact]
    public async Task DeletePostRedirectsToIndex()
    {
        // Arrange
        MetalMock.Setup(s => s.DeleteAsync(1)).Returns(Task.CompletedTask);

        var model = new DeleteMetalViewModel { Id = 1 };

        // Act
        var result = await MetalTypeController.Delete(model);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);

        MetalMock.Verify(s => s.DeleteAsync(1));
    }
}