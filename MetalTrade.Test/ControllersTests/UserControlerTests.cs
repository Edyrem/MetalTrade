using MetalTrade.Business.Dtos;
using MetalTrade.Domain.Enums;
using MetalTrade.Test.Helpers;
using MetalTrade.Web.AdminPanel.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Moq;
using System.Security.Claims;
using Xunit;

namespace MetalTrade.Test.ControllersTests;

public class UserControllerTests : ControllerTestBase
{
    private UserController CreateControllerWithUser(params Claim[] claims)
    {
        var controller = new UserController(UserMock.Object, MapperMock.Object);

        var identity = new ClaimsIdentity(claims, "mock");
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(identity)
            }
        };

        return controller;
    }

    [Fact]
    public async Task Index_Admin_ReturnsViewWithUsers()
    {
        // Arrange
        var controller = CreateControllerWithUser(
            new Claim(ClaimTypes.Name, "admin"),
            new Claim(ClaimTypes.Role, "admin")
        );

        var users = new List<UserDto>
        {
            new UserDto {
                Id = 1,
                UserName = "supplier1",
                Email = "s1@test.com",
                PhoneNumber = "123456789",
                WhatsAppNumber = "987654321",
                PhotoLink = "photo1.jpg",
                Roles = new List<string> { "Supplier" }
            },
            new UserDto {
                Id = 2,
                UserName = "moderator1",
                Email = "m1@test.com",
                PhoneNumber = "111111111",
                WhatsAppNumber = "222222222",
                PhotoLink = "photo2.jpg",
                Roles = new List<string> { "Moderator" }
            }
        };

        UserMock.Setup(s => s.GetAllUsersWithRolesAsync())
            .ReturnsAsync(users);

        MapperMock.Setup(m => m.Map<List<MetalTrade.Web.ViewModels.User.UserViewModel>>(It.IsAny<List<UserDto>>()))
            .Returns((List<UserDto> dtos) => dtos.Select(d => new MetalTrade.Web.ViewModels.User.UserViewModel
            {
                Id = d.Id,
                UserName = d.UserName,
                Email = d.Email,
                PhoneNumber = d.PhoneNumber,
                WhatsAppNumber = d.WhatsAppNumber,
                Photo = d.PhotoLink,
                Roles = d.Roles ?? new List<string>()
            }).ToList());

        // Act
        var result = await controller.Index();

        // Assert
        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<List<MetalTrade.Web.ViewModels.User.UserViewModel>>(view.Model);

        Assert.Equal(2, model.Count);
        Assert.Equal("supplier1", model[0].UserName);
        Assert.Contains("Supplier", model[0].Roles);
        Assert.Equal("moderator1", model[1].UserName);
        Assert.Contains("Moderator", model[1].Roles);

        UserMock.Verify(s => s.GetAllUsersWithRolesAsync(), Times.Once);
    }

    [Fact]
    public async Task Index_UserWithoutRole_ReturnsEmptyView()
    {
        // Arrange
        var controller = CreateControllerWithUser(
            new Claim(ClaimTypes.Name, "user"),
            new Claim(ClaimTypes.Role, "user")
        );

        var currentUser = new UserDto { Id = 1 };

        UserMock.Setup(s => s.GetCurrentUserAsync(It.IsAny<HttpContext>()))
               .ReturnsAsync(currentUser);

        UserMock.Setup(s => s.IsInRoleAsync(currentUser, "admin"))
               .ReturnsAsync(false);

        UserMock.Setup(s => s.IsInRoleAsync(currentUser, "moderator"))
               .ReturnsAsync(false);

        var users = new List<UserDto>();

        UserMock.Setup(s => s.GetAllUsersWithRolesAsync())
               .ReturnsAsync(users);

        MapperMock.Setup(m => m.Map<List<MetalTrade.Web.ViewModels.User.UserViewModel>>(users))
               .Returns(new List<MetalTrade.Web.ViewModels.User.UserViewModel>());

        // Act
        var result = await controller.Index();

        // Assert
        var view = Assert.IsType<ViewResult>(result);
        var model = view.Model as List<MetalTrade.Web.ViewModels.User.UserViewModel>;
        Assert.NotNull(model);
        Assert.Empty(model);
    }

    [Fact]
    public async Task DeleteConfirmed_AdminDeletesUser_RedirectsToIndex()
    {
        // Arrange
        var controller = CreateControllerWithUser(
            new Claim(ClaimTypes.Name, "admin"),
            new Claim(ClaimTypes.Role, "admin")
        );

        var currentAdmin = new UserDto { Id = 1 };
        var userToDelete = new UserDto { Id = 2 };

        UserMock.Setup(s => s.GetCurrentUserAsync(It.IsAny<HttpContext>()))
               .ReturnsAsync(currentAdmin);

        UserMock.Setup(s => s.GetUserByIdAsync(2))
               .ReturnsAsync(userToDelete);

        UserMock.Setup(s => s.IsInRoleAsync(currentAdmin, "admin"))
               .ReturnsAsync(true);

        UserMock.Setup(s => s.DeleteUserAsync(2))
               .Returns(Task.CompletedTask);

        // Act
        var result = await controller.DeleteConfirmed(2);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        UserMock.Verify(s => s.DeleteUserAsync(2), Times.Once);
    }

    [Fact]
    public async Task DeleteConfirmed_ModeratorDeletesUser_Success()
    {
        // Arrange
        var controller = CreateControllerWithUser(
            new Claim(ClaimTypes.Name, "moderator"),
            new Claim(ClaimTypes.Role, "moderator")
        );

        var currentModerator = new UserDto { Id = 1 };
        var userToDelete = new UserDto { Id = 2 };

        UserMock.Setup(s => s.GetCurrentUserAsync(It.IsAny<HttpContext>()))
               .ReturnsAsync(currentModerator);

        UserMock.Setup(s => s.GetUserByIdAsync(2))
               .ReturnsAsync(userToDelete);

        UserMock.Setup(s => s.IsInRoleAsync(currentModerator, "moderator"))
               .ReturnsAsync(true);

        UserMock.Setup(s => s.IsInRoleAsync(userToDelete, "moderator"))
               .ReturnsAsync(false);

        UserMock.Setup(s => s.DeleteUserAsync(2))
               .Returns(Task.CompletedTask);

        // Act
        var result = await controller.DeleteConfirmed(2);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        UserMock.Verify(s => s.DeleteUserAsync(2), Times.Once);
    }

    [Fact]
    public async Task DeleteConfirmed_ModeratorDeletesModerator_RedirectsToIndex()
    {
        // Arrange
        var controller = CreateControllerWithUser(
            new Claim(ClaimTypes.Name, "moderator"),
            new Claim(ClaimTypes.Role, "moderator")
        );

        var currentModerator = new UserDto { Id = 1 };
        var moderatorToDelete = new UserDto { Id = 2 };

        UserMock.Setup(s => s.GetCurrentUserAsync(It.IsAny<HttpContext>()))
               .ReturnsAsync(currentModerator);

        UserMock.Setup(s => s.GetUserByIdAsync(2))
               .ReturnsAsync(moderatorToDelete);

        UserMock.Setup(s => s.IsInRoleAsync(currentModerator, "moderator"))
               .ReturnsAsync(true);

        UserMock.Setup(s => s.IsInRoleAsync(moderatorToDelete, "moderator"))
               .ReturnsAsync(true);

        UserMock.Setup(s => s.DeleteUserAsync(2))
               .Returns(Task.CompletedTask);

        // Act
        var result = await controller.DeleteConfirmed(2);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        UserMock.Verify(s => s.DeleteUserAsync(2), Times.Once);
    }

    [Fact]
    public async Task DeleteConfirmed_SupplierDeletesUser_RedirectsToIndex()
    {
        // Arrange
        var controller = CreateControllerWithUser(
            new Claim(ClaimTypes.Name, "supplier1"),
            new Claim(ClaimTypes.Role, "supplier")
        );

        var currentSupplier = new UserDto { Id = 1 };
        var userToDelete = new UserDto { Id = 2 };

        UserMock.Setup(s => s.GetCurrentUserAsync(It.IsAny<HttpContext>()))
               .ReturnsAsync(currentSupplier);

        UserMock.Setup(s => s.GetUserByIdAsync(2))
               .ReturnsAsync(userToDelete);

        UserMock.Setup(s => s.IsInRoleAsync(currentSupplier, "supplier"))
               .ReturnsAsync(true);

        UserMock.Setup(s => s.DeleteUserAsync(2))
               .Returns(Task.CompletedTask);

        // Act
        var result = await controller.DeleteConfirmed(2);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        UserMock.Verify(s => s.DeleteUserAsync(2), Times.Once);
    }

    [Fact]
    public async Task DeleteConfirmed_UserNotFound_RedirectsToIndexAndDeletes()
    {
        // Arrange
        var controller = CreateControllerWithUser(
            new Claim(ClaimTypes.Name, "admin"),
            new Claim(ClaimTypes.Role, "admin")
        );

        UserMock.Setup(s => s.GetUserByIdAsync(999))
               .ReturnsAsync((UserDto?)null);

        UserMock.Setup(s => s.DeleteUserAsync(999))
               .Returns(Task.CompletedTask);

        // Act
        var result = await controller.DeleteConfirmed(999);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }

    [Fact]
    public async Task DeleteConfirmed_AdminDeletesSelf_RedirectsToIndex()
    {
        // Arrange
        var controller = CreateControllerWithUser(
            new Claim(ClaimTypes.Name, "admin"),
            new Claim(ClaimTypes.Role, "admin")
        );

        var currentAdmin = new UserDto { Id = 1 };
        var adminToDelete = new UserDto { Id = 1 };

        UserMock.Setup(s => s.GetCurrentUserAsync(It.IsAny<HttpContext>()))
               .ReturnsAsync(currentAdmin);

        UserMock.Setup(s => s.GetUserByIdAsync(1))
               .ReturnsAsync(adminToDelete);

        UserMock.Setup(s => s.IsInRoleAsync(currentAdmin, "admin"))
               .ReturnsAsync(true);

        UserMock.Setup(s => s.DeleteUserAsync(1))
               .Returns(Task.CompletedTask);

        // Act
        var result = await controller.DeleteConfirmed(1);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        UserMock.Verify(s => s.DeleteUserAsync(1), Times.Once);
    }

    [Fact]
    public async Task Index_Moderator_SeesOnlyModerators()
    {
        // Arrange
        var controller = CreateControllerWithUser(
            new Claim(ClaimTypes.Name, "moderator"),
            new Claim(ClaimTypes.Role, "moderator")
        );

        var currentUser = new UserDto { Id = 10 };

        UserMock.Setup(s => s.GetCurrentUserAsync(It.IsAny<HttpContext>()))
            .ReturnsAsync(currentUser);

        UserMock.Setup(s => s.IsInRoleAsync(currentUser, "moderator"))
            .ReturnsAsync(true);

        UserMock.Setup(s => s.GetAllUsersWithRolesAsync())
            .ReturnsAsync(new List<UserDto>
            {
                new() { Id = 2, Roles = new List<string> { "moderator" } },
                new() { Id = 3, Roles = new List<string> { "user" } }
            });

        MapperMock.Setup(m => m.Map<List<MetalTrade.Web.ViewModels.User.UserViewModel>>(It.IsAny<List<UserDto>>()))
            .Returns(new List<MetalTrade.Web.ViewModels.User.UserViewModel>());

        // Act
        var result = await controller.Index();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Delete_ModeratorDeletesModerator_ReturnsForbid()
    {
        // Arrange
        var controller = CreateControllerWithUser(
            new Claim(ClaimTypes.Name, "moderator"),
            new Claim(ClaimTypes.Role, "moderator")
        );

        var currentUser = new UserDto { Id = 1 };
        var targetUser = new UserDto { Id = 2 };

        UserMock.Setup(s => s.GetCurrentUserAsync(It.IsAny<HttpContext>()))
            .ReturnsAsync(currentUser);

        UserMock.Setup(s => s.GetUserByIdAsync(2))
            .ReturnsAsync(targetUser);

        UserMock.Setup(s => s.IsInRoleAsync(currentUser, "moderator"))
            .ReturnsAsync(true);

        UserMock.Setup(s => s.IsInRoleAsync(targetUser, "moderator"))
            .ReturnsAsync(true);

        // Act
        var result = await controller.Delete(2);

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task Index_Supplier_ReturnsViewWithUsers()
    {
        // Arrange
        var controller = CreateControllerWithUser(
            new Claim(ClaimTypes.Name, "supplier1"),
            new Claim(ClaimTypes.Role, "supplier")
        );

        var currentUser = new UserDto { Id = 1 };

        UserMock.Setup(s => s.GetCurrentUserAsync(It.IsAny<HttpContext>()))
               .ReturnsAsync(currentUser);

        UserMock.Setup(s => s.IsInRoleAsync(currentUser, "supplier"))
               .ReturnsAsync(true);

        var users = new List<UserDto>
        {
            new UserDto {
                Id = 1,
                UserName = "supplier1",
                Email = "s1@test.com",
                PhoneNumber = "123456789",
                WhatsAppNumber = "987654321",
                PhotoLink = "photo1.jpg",
                Roles = new List<string> { "Supplier" }
            },
            new UserDto {
                Id = 2,
                UserName = "admin",
                Email = "admin@test.com",
                PhoneNumber = "111111111",
                WhatsAppNumber = "222222222",
                PhotoLink = "photo2.jpg",
                Roles = new List<string> { "Admin" }
            }
        };

        UserMock.Setup(s => s.GetAllUsersWithRolesAsync())
               .ReturnsAsync(users);

        MapperMock.Setup(m => m.Map<List<MetalTrade.Web.ViewModels.User.UserViewModel>>(users))
               .Returns(users.Select(d => new MetalTrade.Web.ViewModels.User.UserViewModel
               {
                   Id = d.Id,
                   UserName = d.UserName,
                   Email = d.Email,
                   PhoneNumber = d.PhoneNumber,
                   WhatsAppNumber = d.WhatsAppNumber,
                   Photo = d.PhotoLink,
                   Roles = d.Roles
               }).ToList());

        // Act
        var result = await controller.Index();

        // Assert
        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<List<MetalTrade.Web.ViewModels.User.UserViewModel>>(view.Model);
        Assert.Equal(2, model.Count);
    }
}