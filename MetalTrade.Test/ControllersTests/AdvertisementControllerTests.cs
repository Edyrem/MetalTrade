using MetalTrade.Business.Dtos;
using MetalTrade.Test.Helpers;
using MetalTrade.Web.ViewModels.Advertisement;
using MetalTrade.Web.ViewModels.Product;
using MetalTrade.Web.ViewModels.Promotion;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
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

        // Эмулируем аутентифицированного пользователя
        UserMock
            .Setup(s => s.GetCurrentUserAsync(It.IsAny<HttpContext>()))
            .ReturnsAsync(new UserDto { Id = 1, UserName = "testuser" });

        // Для IsInRolesAsync
        UserMock
            .Setup(s => s.IsInRolesAsync(It.IsAny<UserDto>(), It.IsAny<string[]>()))
            .ReturnsAsync(true);

        // Для AdEndDate
        CommercialMock
            .Setup(s => s.GetActiveAdEndDateAsync(10))
            .ReturnsAsync(DateTime.UtcNow.AddDays(1));

        var controller = AdvertisementController;

        // Мок UrlHelper, чтобы Url.Action не падал
        var mockUrlHelper = new Mock<IUrlHelper>();
        mockUrlHelper
            .Setup(u => u.Action(It.IsAny<UrlActionContext>()))
            .Returns("/mocked-url");

        controller.Url = mockUrlHelper.Object;
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        // Act
        var result = await controller.Details(10);

        // Assert
        var view = Assert.IsType<ViewResult>(result);
        Assert.Equal(vm, view.Model);
        Assert.True((bool)view.ViewData["IsAdmin"]);
        Assert.Equal(1, view.ViewData["CurrentUserId"]);
        Assert.NotNull(view.ViewData["AdEndDate"]);

        AdvertisementMock.Verify(s => s.GetAsync(10), Times.Once);
    }

    [Fact]
    public async Task DetailsUserNotAuthenticatedRedirectsToLogin()
    {
        // Arrange
        AdvertisementMock
            .Setup(s => s.GetAsync(1))
            .ReturnsAsync(new AdvertisementDto { Id = 1 });

        UserMock
            .Setup(s => s.GetCurrentUserAsync(It.IsAny<HttpContext>()))
            .ReturnsAsync((UserDto?)null);

        var controller = AdvertisementController;

        // Мок UrlHelper
        var mockUrlHelper = new Mock<IUrlHelper>();
        mockUrlHelper
            .Setup(u => u.Action(It.IsAny<UrlActionContext>()))
            .Returns("/mocked-url");
        controller.Url = mockUrlHelper.Object;

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        // Act
        var result = await controller.Details(1);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Login", redirect.ActionName);
        Assert.Equal("Account", redirect.ControllerName);
        Assert.NotNull(redirect.RouteValues?["returnUrl"]);
    }

    [Fact]
    public async Task DetailsAdminUserSetsIsAdminTrue()
    {
        var adsDto = new AdvertisementDto { Id = 1 };
        var user = new UserDto { Id = 5 };

        AdvertisementMock.Setup(s => s.GetAsync(1)).ReturnsAsync(adsDto);
        UserMock.Setup(s => s.GetCurrentUserAsync(It.IsAny<HttpContext>()))
                .ReturnsAsync(user);

        UserMock.Setup(s => s.IsInRolesAsync(user, It.IsAny<string[]>()))
                .ReturnsAsync(true);

        CommercialMock.Setup(s => s.GetActiveAdEndDateAsync(1))
                      .ReturnsAsync(DateTime.UtcNow);

        MapperMock.Setup(m => m.Map<AdvertisementViewModel>(adsDto))
                  .Returns(new AdvertisementViewModel());

        var controller = AdvertisementController;

        var result = await controller.Details(1);

        var view = Assert.IsType<ViewResult>(result);
        Assert.True((bool)view.ViewData["IsAdmin"]);
    }

    [Fact]
    public async Task DetailsSetsCurrentUserId()
    {
        var user = new UserDto { Id = 42 };

        AdvertisementMock.Setup(s => s.GetAsync(1))
            .ReturnsAsync(new AdvertisementDto { Id = 1 });

        UserMock.Setup(s => s.GetCurrentUserAsync(It.IsAny<HttpContext>()))
            .ReturnsAsync(user);

        UserMock.Setup(s => s.IsInRolesAsync(user, It.IsAny<string[]>()))
            .ReturnsAsync(false);

        CommercialMock.Setup(s => s.GetActiveAdEndDateAsync(1))
            .ReturnsAsync(DateTime.UtcNow);

        MapperMock.Setup(m => m.Map<AdvertisementViewModel>(It.IsAny<AdvertisementDto>()))
            .Returns(new AdvertisementViewModel());

        var controller = AdvertisementController;

        var result = await controller.Details(1);

        var view = Assert.IsType<ViewResult>(result);
        Assert.Equal(42, view.ViewData["CurrentUserId"]);
    }

    [Fact]
    public async Task DetailsSetsAdEndDate()
    {
        var endDate = DateTime.UtcNow.AddDays(10);

        AdvertisementMock.Setup(s => s.GetAsync(1))
            .ReturnsAsync(new AdvertisementDto { Id = 1 });

        UserMock.Setup(s => s.GetCurrentUserAsync(It.IsAny<HttpContext>()))
            .ReturnsAsync(new UserDto { Id = 1 });

        UserMock.Setup(s => s.IsInRolesAsync(It.IsAny<UserDto>(), It.IsAny<string[]>()))
            .ReturnsAsync(false);

        CommercialMock.Setup(s => s.GetActiveAdEndDateAsync(1))
            .ReturnsAsync(endDate);

        MapperMock.Setup(m => m.Map<AdvertisementViewModel>(It.IsAny<AdvertisementDto>()))
            .Returns(new AdvertisementViewModel());

        var controller = AdvertisementController;

        var result = await controller.Details(1);

        var view = Assert.IsType<ViewResult>(result);
        Assert.Equal(endDate, view.ViewData["AdEndDate"]);
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
        var adsDto = new AdvertisementDto { Id = 1, Title = "Test" };
        var user = new UserDto { Id = 10 };

        AdvertisementMock
            .Setup(s => s.GetAsync(1))
            .ReturnsAsync(adsDto);

        UserMock
            .Setup(s => s.GetCurrentUserAsync(It.IsAny<HttpContext>()))
            .ReturnsAsync(user);

        UserMock
            .Setup(s => s.IsInRolesAsync(user, It.IsAny<string[]>()))
            .ReturnsAsync(false);

        CommercialMock
            .Setup(s => s.GetActiveAdEndDateAsync(1))
            .ReturnsAsync(DateTime.UtcNow.AddDays(5));

        MapperMock
            .Setup(m => m.Map<AdvertisementViewModel>(adsDto))
            .Returns(new AdvertisementViewModel { Id = 1 });

        var controller = AdvertisementController;

        // Act
        var result = await controller.Details(1);

        // Assert
        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<AdvertisementViewModel>(view.Model);
        Assert.Equal(1, model.Id);
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
    public async Task DeleteAdvertisementPhotoAjax_ReturnsSuccessJson()
    {
        // Arrange
        int photoId = 1;
        string photoLink = "link";

        AdvertisementMock
            .Setup(s => s.DeleteAdvertisementPhotoAsync(It.Is<AdvertisementPhotoDto>(
                p => p.Id == photoId && p.PhotoLink == photoLink)))
            .ReturnsAsync(true);

        var controller = AdvertisementController;

        // Act
        var result = await controller.DeleteAdvertisementPhotoAjax(photoId, photoLink);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);

        // ������� �������� success ����� reflection
        var value = jsonResult.Value!;
        var successProp = value.GetType().GetProperty("success");
        Assert.NotNull(successProp);

        var successValue = successProp.GetValue(value);
        Assert.True((bool)successValue);

        AdvertisementMock.Verify(s => s.DeleteAdvertisementPhotoAsync(It.IsAny<AdvertisementPhotoDto>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAdvertisementPhotoAjax_WhenThrowsException_ReturnsFalseJson()
    {
        // Arrange
        int photoId = 1;
        string photoLink = "link";

        AdvertisementMock
            .Setup(s => s.DeleteAdvertisementPhotoAsync(It.IsAny<AdvertisementPhotoDto>()))
            .ThrowsAsync(new Exception("Test exception"));

        var controller = AdvertisementController;

        // Act
        var result = await controller.DeleteAdvertisementPhotoAjax(photoId, photoLink);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);

        // ������� �������� success ����� reflection
        var value = jsonResult.Value!;
        var successProp = value.GetType().GetProperty("success");
        Assert.NotNull(successProp);

        var successValue = successProp.GetValue(value);
        Assert.False((bool)successValue);

        AdvertisementMock.Verify(s => s.DeleteAdvertisementPhotoAsync(It.IsAny<AdvertisementPhotoDto>()), Times.Once);
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
    
    [Fact]
    public async Task ActivateCommercial_ValidModel_ReturnsOk()
    {
        // Arrange
        var model = new PromotionActivateViewModel
        {
            TargetId = 5,
            Days = 7
        };

        CommercialMock
            .Setup(s => s.ActivateAsync(It.IsAny<CommercialDto>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await AdvertisementController.ActivateCommercial(model);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        CommercialMock.Verify(s =>
                s.ActivateAsync(It.Is<CommercialDto>(d =>
                    d.AdvertisementId == 5 && d.StartDate == DateTime.UtcNow && d.EndDate == DateTime.UtcNow.AddDays(7))),
            Times.Once);
    }

    
    [Fact]
    public async Task ActivateCommercial_InvalidModel_ReturnsBadRequest()
    {
        var controller = AdvertisementController;
        // Arrange
        controller.ModelState.AddModelError("Days", "error");

        var model = new PromotionActivateViewModel
        {
            TargetId = 5,
            Days = 0
        };

        // Act
        var result = await controller.ActivateCommercial(model);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task ActivateCommercial_WhenAlreadyActive_ReturnsBadRequest()
    {
        // Arrange
        CommercialMock
            .Setup(s => s.ActivateAsync(It.IsAny<CommercialDto>()))
            .ThrowsAsync(new InvalidOperationException("Реклама уже активна"));

        var model = new PromotionActivateViewModel
        {
            TargetId = 5,
            Days = 7
        };

        // Act
        var result = await AdvertisementController.ActivateCommercial(model);

        // Assert
        var bad = Assert.IsType<BadRequestObjectResult>(result);
    }

    
    [Fact]
    public async Task DeactivateCommercial_ReturnsOk()
    {
        // Arrange
        CommercialMock
            .Setup(s => s.DeactivateAsync(5))
            .Returns(Task.CompletedTask);

        // Act
        var result = await AdvertisementController.DeactivateCommercial(5);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        CommercialMock.Verify(s => s.DeactivateAsync(5), Times.Once);
    }

    [Fact]
    public async Task DeactivateCommercial_WhenNotActive_ReturnsBadRequest()
    {
        // Arrange
        CommercialMock
            .Setup(s => s.DeactivateAsync(It.IsAny<int>()))
            .ThrowsAsync(new InvalidOperationException("Активная реклама не найдена"));

        // Act
        var result = await AdvertisementController.DeactivateCommercial(5);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    
}