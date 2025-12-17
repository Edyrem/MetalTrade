using MetalTrade.Test.Helpers;
using MetalTrade.Web.ViewModels;
using MetalTrade.Web.ViewModel;
using MetalTrade.Business.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using System.Security.Claims;
using Moq;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace MetalTrade.Test.ControllersTests;

public class AccountControllerTests : ControllerTestBase
{    
    private RegisterViewModel ValidRegisterModel() =>
        new()
        {
            UserName = "User1",
            Email = "user@gmail.com",
            PhoneNumber = "500500500",
            WhatsAppNumber = "500500500",
            Password = "Qweqwe1!",
            PasswordConfirm = "Qweqwe1!"
        };
    
    [Fact]
    public async Task RegisterGetReturnsView()
    {
        var result = AccountController.Register();
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task RegisterGetAuthenticatedRedirects()
    {
        var result = AuthenticatedAccountController.Register();

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Advertisement", redirect.ControllerName);
    }

    [Fact]
    public async Task RegisterPostInvalidModelReturnsView()
    {
        var controller = AccountController;
        controller.ModelState.AddModelError("Email", "Required");

        var model = new RegisterViewModel();
        var result = await controller.Register(model);

        var view = Assert.IsType<ViewResult>(result);
        Assert.Equal(model, view.Model);

        UserMock.Verify(
            s => s.CreateUserAsync(It.IsAny<UserDto>(), "user"),
            Times.Never
        );
    }

    [Fact]
    public async Task RegisterPostCreateFailedReturnsView()
    {
        var controller = AccountController;
        UserMock
            .Setup(s => s.CreateUserAsync(It.IsAny<UserDto>(), "user"))
            .ReturnsAsync(false);

        var result = await controller.Register(ValidRegisterModel());

        Assert.IsType<ViewResult>(result);
        Assert.False(controller.ModelState.IsValid);
    }

    [Fact]
    public async Task RegisterPostSuccessLoginRedirects()
    {
        UserMock
            .Setup(s => s.CreateUserAsync(It.IsAny<UserDto>(), "user"))
            .ReturnsAsync(true);

        UserMock
            .Setup(s => s.LoginAsync(It.IsAny<string>(), It.IsAny<string>(), false))
            .ReturnsAsync(SignInResult.Success);

        var result = await AccountController.Register(ValidRegisterModel());

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Advertisement", redirect.ControllerName);
    }
    

    [Fact]
    public async Task LoginPostSuccessRedirects()
    {
        UserMock
            .Setup(s => s.LoginAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(SignInResult.Success);

        var model = new LoginViewModel
        {
            Login = "user",
            Password = "pass",
            RememberMe = false
        };

        var result = await AccountController.Login(model);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Advertisement", redirect.ControllerName);
    }

    [Fact]
    public async Task LogoutAuthenticatedCallsServiceAndRedirects()
    {
        var result = await AuthenticatedAccountController.Logout();

        UserMock.Verify(s => s.LogoutAsync(), Times.Once);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Login", redirect.ActionName);
    }
}
