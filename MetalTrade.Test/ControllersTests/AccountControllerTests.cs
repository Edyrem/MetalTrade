using MetalTrade.Web.Controllers;
using MetalTrade.Business.Interfaces;
using MetalTrade.Web.ViewModels;
using MetalTrade.Web.ViewModel;
using MetalTrade.Business.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using System.Security.Claims;
using AutoMapper;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace MetalTrade.Test.ControllersTests;

public class AccountControllerTests
{
    private Mock<IUserService> _mock;
    private Mock<IMapper> _mockMapper;
    private Mock<IImageUploadService> _mockImageUpload;
    private AccountController _controller;


    private void SetUnauthenticated()
    {
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity())
            }
        };
    }

    private void SetAuthenticated()
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, "user123")
        }, "mock");

        _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);
    }

    private RegisterViewModel ValidRegisterModel() =>
        new()
        {
            UserName = "User1",
            Email = "user@gmail.com",
            PhoneNumber = "500500500",
            WhatsAppNumber = "500500500",
            Password = "Qweqwe1!",
            PasswordConfirm = "Qweqwe1"
        };

    public AccountControllerTests()
    {
        _mock = new Mock<IUserService>();
        _mockMapper = new Mock<IMapper>();
        _mockImageUpload = new Mock<IImageUploadService>();

        _controller = new AccountController(
            _mock.Object,
            _mockMapper.Object,
            _mockImageUpload.Object
        );

        SetUnauthenticated();
    }

    

    [Fact]
    public void RegisterGetReturnsView()
    {
        var result = _controller.Register();
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void RegisterGetAuthenticatedRedirects()
    {
        SetAuthenticated();

        var result = _controller.Register();

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Advertisement", redirect.ControllerName);
    }

    [Fact]
    public async Task RegisterPostInvalidModelReturnsView()
    {
        _controller.ModelState.AddModelError("Email", "Required");

        var model = new RegisterViewModel();
        var result = await _controller.Register(model);

        var view = Assert.IsType<ViewResult>(result);
        Assert.Equal(model, view.Model);

        _mock.Verify(s => s.CreateUserAsync(It.IsAny<UserDto>(), "user"), Times.Never);
    }

    [Fact]
    public async Task RegisterPostCreateFailedReturnsView()
    {
        _mock.Setup(s => s.CreateUserAsync(It.IsAny<UserDto>(), "user"))
            .ReturnsAsync(false);

        var model = ValidRegisterModel();
        var result = await _controller.Register(model);

        Assert.IsType<ViewResult>(result);
        Assert.False(_controller.ModelState.IsValid);
    }

    [Fact]
    public async Task RegisterPostSuccessLoginRedirects()
    {
        _mock.Setup(s => s.CreateUserAsync(It.IsAny<UserDto>(), "user"))
            .ReturnsAsync(true);

        _mock.Setup(s => s.LoginAsync(It.IsAny<string>(), It.IsAny<string>(), false))
            .ReturnsAsync(SignInResult.Success);

        var model = ValidRegisterModel();

        var result = await _controller.Register(model);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Advertisement", redirect.ControllerName);

        _mock.Verify(s => s.CreateUserAsync(It.IsAny<UserDto>(), "user"));
        _mock.Verify(s => s.LoginAsync(model.UserName, model.Password, false));
    }

    [Fact]
    public async Task RegisterPostLoginFailedReturnsView()
    {
        _mock.Setup(s => s.CreateUserAsync(It.IsAny<UserDto>(), "user"))
            .ReturnsAsync(true);

        _mock.Setup(s => s.LoginAsync(It.IsAny<string>(), It.IsAny<string>(), false))
            .ReturnsAsync(SignInResult.Failed);

        var model = ValidRegisterModel();

        var result = await _controller.Register(model);

        Assert.IsType<ViewResult>(result);
        Assert.False(_controller.ModelState.IsValid);
    }
    

    [Fact]
    public void LoginGetReturnsView()
    {
        var result = _controller.Login();
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void LoginGetAuthenticatedRedirects()
    {
        SetAuthenticated();

        var result = _controller.Login();

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Advertisement", redirect.ControllerName);
    }
    

    [Fact]
    public async Task LoginPostInvalidModelReturnsView()
    {
        _controller.ModelState.AddModelError("Login", "Required");

        var model = new LoginViewModel();
        var result = await _controller.Login(model);

        Assert.IsType<ViewResult>(result);

        _mock.Verify(s => s.LoginAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
    }

    [Fact]
    public async Task LoginPostFailedReturnsView()
    {
        _mock.Setup(s => s.LoginAsync("user", "pass", false))
            .ReturnsAsync(SignInResult.Failed);

        var model = new LoginViewModel
        {
            Login = "user",
            Password = "pass",
            RememberMe = false
        };

        var result = await _controller.Login(model);

        Assert.IsType<ViewResult>(result);
        Assert.False(_controller.ModelState.IsValid);
    }

    [Fact]
    public async Task LoginPostSuccessRedirects()
    {
        _mock.Setup(s => s.LoginAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(SignInResult.Success);

        var model = new LoginViewModel
        {
            Login = "user",
            Password = "pass",
            RememberMe = false
        };

        var result = await _controller.Login(model);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Advertisement", redirect.ControllerName);
    }
    

    [Fact]
    public async Task LogoutNotAuthenticatedRedirects()
    {
        SetUnauthenticated();

        var result = await _controller.Logout();

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }

    [Fact]
    public async Task LogoutAuthenticatedCallsServiceAndRedirects()
    {
        SetAuthenticated();

        var result = await _controller.Logout();

        _mock.Verify(s => s.LogoutAsync(), Times.Once);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Login", redirect.ActionName);
    }
}
