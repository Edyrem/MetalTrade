using MetalTrade.Business.Dtos;
using MetalTrade.Domain.Entities;
using MetalTrade.Domain.Enums;
using MetalTrade.Test.Helpers;
using MetalTrade.Web.ViewModels.Advertisement;
using MetalTrade.Web.ViewModels.Product;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using Xunit;

namespace MetalTrade.Test.ControllersTests;

public class AdvertisementControllerTests : ControllerTestBase
{
    
    [Fact]
    public async Task IndexReturnsViewWithModel()
    {
        // Arrange
        var adsDtos = new List<AdvertisementDto>
        {
            new() { Id = 1, Title = "Test" },
            new() { Id = 2, Title = "Test2" }
        };
        var advertisementViewModels = new List<AdvertisementViewModel>
        {
            new() { Id = 1, Title = "Test" },
            new() { Id = 2, Title = "Test2" }
        };

        AdvertisementMock.Setup(s => s.GetFilteredAsync(It.IsAny<AdvertisementFilterDto>()))
            .ReturnsAsync(adsDtos);
        MapperMock.Setup(m => m.Map<List<AdvertisementViewModel>>(adsDtos))
            .Returns(advertisementViewModels);

        // Act
        var result = await AdvertisementController.Index();

        // Assert
        var view = Assert.IsType<ViewResult>(result);
        Assert.IsType<List<AdvertisementViewModel>>(view.Model);

        AdvertisementMock.Verify(s => s.GetFilteredAsync(It.IsAny<AdvertisementFilterDto>()));  
    }
    
    [Fact]
    public async Task PartialListReturnsPartialViewWithModel()
    {
        // Arrange
        var adsDtos = new List<AdvertisementDto> 
        { 
            new() { Id = 1, Title = "Test"}, 
            new() { Id = 2, Title = "Test2" } 
        };
        var advertisementViewModels = new List<AdvertisementViewModel> 
        { 
            new() { Id = 1, Title = "Test" }, 
            new() { Id = 2, Title = "Test2" } 
        };
        
        AdvertisementMock
            .Setup(s => s.GetFilteredAsync(It.IsAny<AdvertisementFilterDto>()))
            .ReturnsAsync(adsDtos);
        MapperMock
            .Setup(m => m.Map<List<AdvertisementViewModel>>(It.IsAny<List<AdvertisementDto>>()))
            .Returns(advertisementViewModels);

        // Act
        var result = await AdvertisementController.PartialList(new AdvertisementFilterDto());

        // Assert
        var view = Assert.IsType<PartialViewResult>(result);
        Assert.Equal("_AdsGrid", view.ViewName);
        Assert.IsType<List<AdvertisementViewModel>>(view.Model);

        AdvertisementMock.Verify(s => s.GetFilteredAsync(It.IsAny<AdvertisementFilterDto>()));
    }
    
    [Fact]
    public async Task DetailsWhenFoundReturnsViewWithModel()
    {
        // Arrange
        var dto = new AdvertisementDto { Id = 10, Title = "Test" };
        var vm = new AdvertisementViewModel { Id = 10, Title = "Test" };

        AdvertisementMock
            .Setup(s => s.GetAsync(10))
            .ReturnsAsync(dto);
        MapperMock
            .Setup(m => m.Map<AdvertisementViewModel>(dto))
            .Returns(vm);

        // Act
        var result = await AdvertisementController.Details(10);

        // Assert
        var view = Assert.IsType<ViewResult>(result);
        Assert.Equal(vm, view.Model);

        AdvertisementMock.Verify(s => s.GetAsync(10));
    }

    [Fact]
    public async Task DetailsWhenNotFoundRedirectsToIndex()
    {
        // Arrange
        AdvertisementMock
            .Setup(s => s.GetAsync(10))
            .ReturnsAsync((AdvertisementDto?)null);

        // Act
        var result = await AdvertisementController.Details(10);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }

    [Fact]
    public async Task CreateGetReturnsViewWithProducts()
    {
        // Arrange
        var productDtos = new List<ProductDto>
        {
            new() { Id = 1, Name = "Test" }
        };
        var vm = new List<ProductViewModel>
        {
            new() { Id = 1, Name = "Test" }
        };

        ProductMock.Setup(s => s.GetAllAsync()).ReturnsAsync(productDtos);
        MapperMock.Setup(m => m.Map<List<ProductViewModel>>(productDtos))
            .Returns(vm);

        // Act
        var result = await AdvertisementController.Create();

        // Assert
        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<CreateAdvertisementViewModel>(view.Model);
        Assert.Equal(vm, model.Products);

        ProductMock.Verify(s => s.GetAllAsync());
    }

    [Fact]
    public async Task CreatePostValidRedirectsToIndex()
    {
        // Arrange
        var model = new CreateAdvertisementViewModel();

        UserMock
            .Setup(s => s.GetCurrentUserAsync(It.IsAny<HttpContext>()))
            .ReturnsAsync(new UserDto { Id = 5 });

        MapperMock
            .Setup(m => m.Map<AdvertisementDto>(model))
            .Returns(new AdvertisementDto());

        // Act
        var result = await AdvertisementController.Create(model);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);

        AdvertisementMock.Verify(s => s.CreateAsync(It.IsAny<AdvertisementDto>()));
    }

    [Fact]
    public async Task CreatePostUserNullReturnsForbid()
    {
        // Arrange
        UserMock
            .Setup(s => s.GetCurrentUserAsync(It.IsAny<HttpContext>()))
            .ReturnsAsync((UserDto?)null);

        var model = new CreateAdvertisementViewModel();

        // Act
        var result = await AdvertisementController.Create(model);

        // Assert
        Assert.IsType<ForbidResult>(result);
    }
    

    [Fact]
    public async Task DetailsFoundReturnsViewWithModel()
    {
        // Arrange
        var dto = new AdvertisementDto { Id = 1, Title = "Test" };

        AdvertisementMock
            .Setup(s => s.GetAsync(1))
            .ReturnsAsync(dto);

        MapperMock
            .Setup(m => m.Map<AdvertisementViewModel>(dto))
            .Returns(new AdvertisementViewModel { Id = 1 });

        // Act
        var result = await AdvertisementController.Details(1);

        // Assert
        var view = Assert.IsType<ViewResult>(result);
        Assert.IsType<AdvertisementViewModel>(view.Model);
    }

    [Fact]
    public async Task DetailsNotFoundReturnsNotFound()
    {
        // Arrange
        AdvertisementMock
            .Setup(s => s.GetAsync(It.IsAny<int>()))
            .ReturnsAsync((AdvertisementDto?)null);

        // Act
        var result = await AdvertisementController.Details(100);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }
    
    [Fact]
    public async Task EditGetFoundReturnsViewWithModel()
    {
        // Arrange
        var controller = AdvertisementController;
        var dto = new AdvertisementDto { Id = 2, Title = "Test", UserId = 5 };

        AdvertisementMock
            .Setup(s => s.GetAsync(2))
            .ReturnsAsync(dto);
        
        UserMock
            .Setup(s => s.GetCurrentUserAsync(It.IsAny<HttpContext>()))
            .ReturnsAsync(new UserDto { Id = 5 });
        
        UserMock
            .Setup(s => s.IsInRoleAsync(It.IsAny<UserDto>(), "admin"))
            .ReturnsAsync(true);

        MapperMock
            .Setup(m => m.Map<EditAdvertisementViewModel>(dto))
            .Returns(new EditAdvertisementViewModel { Id = 2 });

        ProductMock
            .Setup(s => s.GetAllAsync())
            .ReturnsAsync(new List<ProductDto>());

        MapperMock
            .Setup(m => m.Map<List<ProductViewModel>>(It.IsAny<List<ProductDto>>()))
            .Returns(new List<ProductViewModel>());

        // Act
        var result = await controller.Edit(2, null);

        // Assert
        var view = Assert.IsType<ViewResult>(result);
        Assert.IsType<EditAdvertisementViewModel>(view.Model);
    }
    
    [Fact]
    public async Task EditGetNotFoundRedirectsToIndex()
    {
        // Arrange
        var controller = AdvertisementController;

        UserMock
            .Setup(s => s.GetCurrentUserAsync(It.IsAny<HttpContext>()))
            .ReturnsAsync(new UserDto { Id = 5 });

        UserMock
            .Setup(s => s.IsInRoleAsync(It.IsAny<UserDto>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        AdvertisementMock
            .Setup(s => s.GetAsync(It.IsAny<int>()))
            .ReturnsAsync((AdvertisementDto?)null);

        // Act
        var result = await controller.Edit(222, null);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }

    [Fact]
    public async Task EditPostInvalidReturnsView()
    {
        // Arrange
        AdvertisementController.ModelState.AddModelError("x", "error");

        var user = new UserDto { Id = 10 };

        UserMock
            .Setup(s => s.GetCurrentUserAsync(It.IsAny<HttpContext>()))
            .ReturnsAsync(user);

        UserMock
            .Setup(s => s.IsInRoleAsync(user, "admin"))
            .ReturnsAsync(false);

        UserMock
            .Setup(s => s.IsInRoleAsync(user, "moderator"))
            .ReturnsAsync(false);

        var model = new EditAdvertisementViewModel
        {
            Id = 1,
            UserId = 99
        };

        ProductMock
            .Setup(s => s.GetAllAsync())
            .ReturnsAsync(new List<ProductDto>());

        AdvertisementMock
            .Setup(s => s.GetAsync(It.IsAny<int>()))
            .ReturnsAsync(new AdvertisementDto { Photoes = new List<AdvertisementPhotoDto>() });

        // Act
        var result = await AdvertisementController.Edit(model);

        // Assert
        var view = Assert.IsType<ViewResult>(result);
        Assert.IsAssignableFrom<EditAdvertisementViewModel>(view.Model);

        AdvertisementMock.Verify(s => s.UpdateAsync(It.IsAny<AdvertisementDto>()), Times.Never);
    }
    

    [Fact]
    public async Task EditPostValidRedirects()
    {
        // Arrange
        var user = new UserDto() { Id = 10 };

        UserMock
            .Setup(s => s.GetCurrentUserAsync(It.IsAny<HttpContext>()))
            .ReturnsAsync(user);

        UserMock
            .Setup(s => s.IsInRoleAsync(user, "admin"))
            .ReturnsAsync(false);

        UserMock
            .Setup(s => s.IsInRoleAsync(user, "moderator"))
            .ReturnsAsync(false);

        var model = new EditAdvertisementViewModel
        {
            Id = 3,
            UserId = 10,
        };

        MapperMock
            .Setup(m => m.Map<AdvertisementDto>(model))
            .Returns(new AdvertisementDto { Id = 3 });

        AdvertisementMock
            .Setup(s => s.UpdateAsync(It.IsAny<AdvertisementDto>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await AdvertisementController.Edit(model);

        // Assert
        var redirect = Assert.IsAssignableFrom<RedirectToActionResult>(result);
        Assert.Equal("Details", redirect.ActionName);
        Assert.Equal(3, redirect.RouteValues!["id"]);
    }
    
    [Fact]
    public async Task DeleteConfirmedRedirects()
    {
        // Arrange
        var user = new UserDto() { Id = 10 };

        UserMock
            .Setup(s => s.GetCurrentUserAsync(It.IsAny<HttpContext>()))
            .ReturnsAsync(user);

        UserMock
            .Setup(s => s.IsInRoleAsync(user, "admin"))
            .ReturnsAsync(false);

        UserMock
            .Setup(s => s.IsInRoleAsync(user, "moderator"))
            .ReturnsAsync(false);

        var model = new DeleteAdvertisementViewModel
        {
            Id = 5,
            UserId = 10
        };

        AdvertisementMock
            .Setup(s => s.DeleteAsync(5))
            .Returns(Task.CompletedTask);

        // Act
        var result = await AdvertisementController.Delete(model);

        // Assert
        var redirect = Assert.IsAssignableFrom<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);

        AdvertisementMock.Verify(s => s.DeleteAsync(5));
    }

    [Fact]
    public async Task DeleteGetWhenUserNullReturnsForbid()
    {
        // Arrange
        UserMock
            .Setup(s => s.GetCurrentUserAsync(It.IsAny<HttpContext>()))
            .ReturnsAsync((UserDto?)null);

        // Act
        var result = await AdvertisementController.Delete(5);

        // Assert
        Assert.IsAssignableFrom<ForbidResult>(result);
    }

    [Fact]
    public async Task PartialListReturnsPartialViewWithFilteredAds()
    {
        // Arrange
        var adsDtos = new List<AdvertisementDto>
        {
            new() { Id = 1, Title = "Test" },
            new() { Id = 2, Title = "Test2" }
        };
        var filter = new AdvertisementFilterDto();
        
        AdvertisementMock.Setup(s => s.GetFilteredAsync(filter))
            .ReturnsAsync(adsDtos);
        
        MapperMock
            .Setup(m => m.Map<List<AdvertisementViewModel>>(It.IsAny<List<AdvertisementDto>>()))
            .Returns(new List<AdvertisementViewModel>());

        UserMock.Setup(u => u.GetCurrentUserAsync(It.IsAny<HttpContext>()))
            .ReturnsAsync(new UserDto { Id = 5 });

        // Act
        var result = await AdvertisementController.PartialList(filter);

        // Assert
        var partial = Assert.IsType<PartialViewResult>(result);
        Assert.Equal("_AdsGrid", partial.ViewName);
        Assert.IsType<List<AdvertisementViewModel>>(partial.Model);

        AdvertisementMock.Verify(s => s.GetFilteredAsync(filter), Times.Once);
    }

    [Fact]
    public async Task DeleteAdvertisementPhotoRedirectsBackToEdit()
    {
        // Arrange
        int advertisementId = 10;

        // Act
        var result = await AdvertisementController.DeleteAdvertisementPhoto(1, "link", advertisementId);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Edit", redirect.ActionName);
        Assert.Equal(advertisementId, redirect.RouteValues["Id"]);
    }
    
    [Fact]
    public async Task ApproveAdvertisementRedirectsToIndex()
    {
        // Arrange
        int adId = 5;

        // Act
        var result = await AdvertisementController.ApproveAdvertisement(adId);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);

        AdvertisementMock.Verify(s => s.ApproveAsync(adId), Times.Once);
    }

    [Fact]
    public async Task ApproveAdvertisementAddsModelErrorOnException()
    {
        // Arrange
        AdvertisementMock.Setup(s => s.ApproveAsync(It.IsAny<int>()))
            .ThrowsAsync(new Exception("error"));
        
        var controller = AdvertisementController;
        
        // Act
        var result = await controller.ApproveAdvertisement(1);

        // Assert
        Assert.False(controller.ModelState.IsValid);
    }
    
    [Fact]
    public async Task RejectAdvertisementRedirectsToIndex()
    {
        // Arrange
        int adId = 5;
        
        // Act
        var result = await AdvertisementController.RejectAdvertisement(adId);
        
        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        AdvertisementMock.Verify(s => s.RejectAsync(adId), Times.Once);
    }

    [Fact]
    public async Task RejectAdvertisementAddsModelErrorOnException()
    {
        // Arrange
        AdvertisementMock.Setup(s => s.RejectAsync(It.IsAny<int>()))
            .ThrowsAsync(new Exception("error"));

        var controller = AdvertisementController;

        // Act
        var result = await controller.RejectAdvertisement(1);
        
        // Assert
        Assert.False(controller.ModelState.IsValid);
    }

    
    [Fact]
    public async Task ArchiveAdvertisementAddsModelErrorOnException()
    {
        // Arrange
        AdvertisementMock.Setup(s => s.ArchiveAsync(It.IsAny<int>()))
            .ThrowsAsync(new Exception("error"));

        var controller = AdvertisementController;

        // Act
        var result = await controller.ArchiveAdvertisement(1);
        
        // Assert
        Assert.False(controller.ModelState.IsValid);
    }

    [Fact]
    public async Task ArchiveAdvertisementRedirectsToIndex()
    {
        // Arrange
        int adId = 5;
        
        // Act
        var result = await AdvertisementController.ArchiveAdvertisement(adId);
        
        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        AdvertisementMock.Verify(s => s.ArchiveAsync(adId), Times.Once);
    }
    
    [Fact]
    public async Task CreatePostOnExceptionThrowsException()
    {
        // Arrange
        UserMock
            .Setup(s => s.GetCurrentUserAsync(It.IsAny<HttpContext>()))
            .ReturnsAsync(new UserDto { Id = 5 });

        MapperMock
            .Setup(m => m.Map<AdvertisementDto>(It.IsAny<CreateAdvertisementViewModel>()))
            .Returns(new AdvertisementDto());

        AdvertisementMock
            .Setup(s => s.CreateAsync(It.IsAny<AdvertisementDto>()))
            .ThrowsAsync(new Exception("fail"));

        var model = new CreateAdvertisementViewModel();

        // Act & Assert

        await Assert.ThrowsAsync<Exception>(() =>
            AdvertisementController.Create(model));
    }
    
    [Fact]
    public async Task CreatePostUserAuthenticatedReturnsRedirect()
    {
        // Arrange
        UserMock
            .Setup(s => s.GetCurrentUserAsync(It.IsAny<HttpContext>()))
            .ReturnsAsync(new UserDto { Id = 99 });

        MapperMock
            .Setup(m => m.Map<AdvertisementDto>(It.IsAny<CreateAdvertisementViewModel>()))
            .Returns(new AdvertisementDto());

        AdvertisementMock
            .Setup(s => s.CreateAsync(It.IsAny<AdvertisementDto>()))
            .Returns(Task.CompletedTask);

        var model = new CreateAdvertisementViewModel();

        // Act
        var result = await AdvertisementController.Create(model);

        // Assert
        Assert.IsType<RedirectToActionResult>(result);
    }
    

    [Fact]
    public async Task EditPostUserNullReturnsForbid()
    {
        // Arrange
        UserMock
            .Setup(s => s.GetCurrentUserAsync(It.IsAny<HttpContext>()))
            .ReturnsAsync((UserDto?)null);

        var model = new EditAdvertisementViewModel();

        // Act
        var result = await AdvertisementController.Edit(model);

        // Assert
        Assert.IsType<ForbidResult>(result);
    }
    
    [Fact]
    public async Task EditPostOnExceptionThrowsException()
    {
        // Arrange
        var model = new EditAdvertisementViewModel
        {
            Id = 3,
            UserId = 10
        };

        UserMock
            .Setup(s => s.GetCurrentUserAsync(It.IsAny<HttpContext>()))
            .ReturnsAsync(new UserDto { Id = 10 });

        AdvertisementMock
            .Setup(s => s.UpdateAsync(It.IsAny<AdvertisementDto>()))
            .ThrowsAsync(new Exception("fail"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() =>
            AdvertisementController.Edit(model));
    }

    [Fact]
    public async Task EditPostDtoNotFoundReturnsView()
    {
        // Arrange
        var model = new EditAdvertisementViewModel { Id = 3 };

        UserMock
            .Setup(s => s.GetCurrentUserAsync(It.IsAny<HttpContext>()))
            .ReturnsAsync(new UserDto { Id = 10 });

        AdvertisementMock
            .Setup(s => s.GetAsync(model.Id))
            .ReturnsAsync((AdvertisementDto?)null);

        // Act
        var result = await AdvertisementController.Edit(model);

        // Assert
        Assert.IsType<ViewResult>(result);
    }


    [Fact]
    public async Task DeletePostUserNullReturnsForbid()
    {
        // Arrange
        UserMock
            .Setup(s => s.GetCurrentUserAsync(It.IsAny<HttpContext>()))
            .ReturnsAsync((UserDto?)null);

        var model = new DeleteAdvertisementViewModel();

        // Act
        var result = await AdvertisementController.Delete(model);

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task DeletePostDtoNotFoundRedirectsToIndex()
    {
        // Arrange
        var model = new DeleteAdvertisementViewModel { Id = 5 };
    
        UserMock
            .Setup(s => s.GetCurrentUserAsync(It.IsAny<HttpContext>()))
            .ReturnsAsync(new UserDto { Id = 10 });

        AdvertisementMock
            .Setup(s => s.DeleteAsync(model.Id))
            .ThrowsAsync(new KeyNotFoundException());

        // Act
        var result = await AdvertisementController.Delete(model);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }

    
    [Fact]
    public async Task DeletePostOnExceptionThrowsException()
    {
        // Arrange
        var model = new DeleteAdvertisementViewModel
        {
            Id = 5,
            UserId = 10
        };

        UserMock
            .Setup(s => s.GetCurrentUserAsync(It.IsAny<HttpContext>()))
            .ReturnsAsync(new UserDto { Id = 10 });

        AdvertisementMock
            .Setup(s => s.DeleteAsync(model.Id))
            .ThrowsAsync(new Exception("fail"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() =>
            AdvertisementController.Delete(model));
    }
    
}