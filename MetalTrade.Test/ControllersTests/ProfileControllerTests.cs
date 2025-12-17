using System.Security.Claims;
using AutoMapper;
using MetalTrade.Business.Dtos;
using MetalTrade.Business.Interfaces;
using MetalTrade.Domain.Entities;
using MetalTrade.Web.Controllers;
using MetalTrade.Web.ViewModels;
using MetalTrade.Web.ViewModels.Profile;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace MetalTrade.Test.ControllersTests;

public class ProfileControllerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IWebHostEnvironment> _envMock;
    private ProfileController _controller;
    
    public ProfileControllerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _mapperMock = new Mock<IMapper>();
        _envMock = new Mock<IWebHostEnvironment>();
        _controller = new ProfileController(_userServiceMock.Object, _mapperMock.Object, _envMock.Object);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }
    
    private void SetAuthenticated()
    {
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "testuser") }, "mock");
        _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);
    }
    
    
    [Fact]
    public async Task IndexWhenUserIsNullReturnNotFound()
    { 
        // Arrange
        _userServiceMock.Setup(s => s.GetCurrentUserAsync(It.IsAny<HttpContext>())).ReturnsAsync((UserDto?)null);
        
        // Act
        var result = await _controller.Index();

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
    
    [Fact]
    public async Task IndexUserFoundReturnsViewWithUsers()
    {
        // Arrange
        SetAuthenticated();
        var userDto = new UserDto { Id = 1 };
        var viewModel = new UserProfileWithAdsViewModel();

        _userServiceMock.Setup(s => s.GetCurrentUserAsync(It.IsAny<HttpContext>())).ReturnsAsync(userDto);
        _userServiceMock.Setup(s => s.GetUserWithAdvertisementByIdAsync(1)).ReturnsAsync(userDto);
        _userServiceMock.Setup(s => s.IsInRoleAsync(userDto, "supplier")).ReturnsAsync(true);
        _mapperMock.Setup(S => S.Map<UserProfileWithAdsViewModel>(userDto)).Returns(viewModel);

        // Act
        var result = await _controller.Index();

        // Assert
        Assert.IsType<ViewResult>(result);
    }
    
    [Fact]
    public async Task IndexReturnsViewWithModel()
    {
        // Arrange
        SetAuthenticated();
        var userDto = new UserDto { Id = 1 };
        var vm = new UserProfileWithAdsViewModel();

        _userServiceMock.Setup(s => s.GetCurrentUserAsync(It.IsAny<HttpContext>())).ReturnsAsync(userDto);
        _userServiceMock.Setup(s => s.GetUserWithAdvertisementByIdAsync(1)).ReturnsAsync(userDto);
        _userServiceMock.Setup(s => s.IsInRoleAsync(userDto, "supplier")).ReturnsAsync(false);
        _mapperMock.Setup(m => m.Map<UserProfileWithAdsViewModel>(userDto)).Returns(vm);

        // Act
        var result = await _controller.Index() as ViewResult;

        // Assert
        Assert.NotNull(result!.Model);
    }
    
    
    [Fact]
    public async Task EditGetReturnsView()
    {
        // Arrange
        var userDto = new UserDto();
        var vm = new UserProfileEditViewModel();

        _userServiceMock.Setup(s => s.GetCurrentUserAsync(It.IsAny<HttpContext>())).ReturnsAsync(userDto);
        _mapperMock.Setup(m => m.Map<UserProfileEditViewModel>(userDto)).Returns(vm);

        // Act
        var result = await _controller.Edit();

        // Assert
        Assert.IsType<ViewResult>(result);
    }
    
    [Fact]
    public async Task EditGetReturnsViewWithModel()
    {
        // Arrange
        var userDto = new UserDto();
        var editVm = new UserProfileEditViewModel();

        _userServiceMock.Setup(s => s.GetCurrentUserAsync(It.IsAny<HttpContext>())).ReturnsAsync(userDto);
        _mapperMock.Setup(m => m.Map<UserProfileEditViewModel>(userDto)).Returns(editVm);

        // Act
        var result = await _controller.Edit() as ViewResult;

        // Assert
        Assert.NotNull(result!.Model);
    }
    
    [Fact]
    public async Task EditPostUserIsNullReturnsNotFound()
    {
        // Arrange
        _userServiceMock.Setup(s => s.GetCurrentUserAsync(It.IsAny<HttpContext>())).ReturnsAsync((UserDto?)null);

        // Act
        var result = await _controller.Edit(new UserProfileEditViewModel());

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
    
    [Fact]
    public async Task EditPostInvalidModelReturnsView()
    {
        // Arrange
        _controller.ModelState.AddModelError("x", "error");
        _userServiceMock.Setup(s => s.GetCurrentUserAsync(It.IsAny<HttpContext>())).ReturnsAsync(new UserDto());

        // Act
        var result = await _controller.Edit(new UserProfileEditViewModel());

        // Assert
        Assert.IsType<ViewResult>(result);
    }
    
    [Fact]
    public async Task EditPostValidModelRedirectsToIndex()
    {
        // Arrange
        var model = new UserProfileEditViewModel();
        var userDto = new UserDto();

        _userServiceMock.Setup(s => s.GetCurrentUserAsync(It.IsAny<HttpContext>())).ReturnsAsync(userDto);
        _mapperMock.Setup(m => m.Map<UserDto>(model)).Returns(userDto);

        // Act
        var result = await _controller.Edit(model);

        // Assert
        Assert.IsType<RedirectToActionResult>(result);
    }
    
    [Fact]
    public void ChangePasswordGetReturnsView()
    {
        // Act
        var result = _controller.ChangePassword();

        // Assert
        Assert.IsType<ViewResult>(result);
    }
    
    [Fact]
    public async Task ChangePasswordPostInvalidModelReturnsView()
    {
        // Arrange
        _controller.ModelState.AddModelError("", "error");

        // Act
        var result = await _controller.ChangePassword(new ChangePasswordViewModel());

        // Assert
        Assert.IsType<ViewResult>(result);
    }
    
    [Fact]
    public async Task ChangePasswordPostUserNullReturnsNotFound()
    {
        // Arrange
        _userServiceMock.Setup(s => s.GetCurrentUserAsync(It.IsAny<HttpContext>())).ReturnsAsync((UserDto?)null);

        // Act
        var result = await _controller.ChangePassword(new ChangePasswordViewModel());

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
    
    [Fact]
    public async Task ChangePasswordPostFailedReturnsView()
    {
        // Arrange
        var user = new UserDto();

        _userServiceMock.Setup(s => s.GetCurrentUserAsync(It.IsAny<HttpContext>())).ReturnsAsync(user);
        _userServiceMock.Setup(s => s.ChangePasswordAsync(user, It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed());

        // Act
        var result = await _controller.ChangePassword(new ChangePasswordViewModel());

        // Assert
        Assert.IsType<ViewResult>(result);
    }
    
    
    [Fact]
    public async Task ChangePasswordPostSuccessRedirectsToIndex()
    {
        // Arrange
        var user = new UserDto();

        _userServiceMock.Setup(s => s.GetCurrentUserAsync(It.IsAny<HttpContext>())).ReturnsAsync(user);
        _userServiceMock.Setup(s => s.ChangePasswordAsync(user, It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _controller.ChangePassword(new ChangePasswordViewModel());

        // Assert
        Assert.IsType<RedirectToActionResult>(result);
    }
    
    
}