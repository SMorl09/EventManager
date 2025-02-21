using Application.DTO.Request;
using Application.DTO.Response;
using Application.Interface;
using EventManager.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEventManager
{
    public class UserControllerIntegrationTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly UsersController _usersController;

        public UserControllerIntegrationTests()
        {
            _mockUserService = new Mock<IUserService>();
            _usersController = new UsersController(_mockUserService.Object);
        }

        [Fact]
        public async Task GetUserById_ExistingId_ReturnsOk()
        {
            // Arrange
            var userId = 1;
            var userResponse = new UserResponse { Id = userId, Name = "John", Surename = "Doe", Email = "john.doe@example.com" };
            _mockUserService.Setup(service => service.GetUserByIdAsync(userId)).ReturnsAsync(userResponse);

            // Act
            var result = await _usersController.GetUserById(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<UserResponse>(okResult.Value);
            Assert.Equal(userId, returnValue.Id);
        }

        [Fact]
        public async Task GetUserById_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var userId = 1;
            _mockUserService.Setup(service => service.GetUserByIdAsync(userId)).ReturnsAsync((UserResponse)null);

            // Act
            var result = await _usersController.GetUserById(userId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateUser_ValidRequest_ReturnsCreatedAtAction()
        {
            // Arrange
            var userRequest = new UserRequest { Name = "Jane", Surename = "Doe", Email = "jane.doe@example.com", Password = "password123", Role = "User" };
            var userResponse = new UserResponse { Id = 1, Name = userRequest.Name, Surename = userRequest.Surename, Email = userRequest.Email };
            _mockUserService.Setup(service => service.CreateUserAsync(userRequest)).ReturnsAsync(userResponse);

            // Act
            var result = await _usersController.CreateUser(userRequest);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnValue = Assert.IsType<UserResponse>(createdAtActionResult.Value);
            Assert.Equal(userRequest.Name, returnValue.Name);
        }

        [Fact]
        public async Task UpdateUser_ExistingId_ValidRequest_ReturnsNoContent()
        {
            // Arrange
            var userId = 1;
            var userRequest = new UserRequest { Name = "John Updated", Surename = "Doe Updated", Email = "john.updated@example.com", Password = "newpassword123", Role = "Admin" };

            // Act
            var result = await _usersController.UpdateUser(userId, userRequest);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockUserService.Verify(service => service.UpdateUserAsync(userId, userRequest), Times.Once);
        }

        [Fact]
        public async Task UpdateUser_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var userId = 1;
            var userRequest = new UserRequest { Name = "John Updated", Surename = "Doe Updated", Email = "john.updated@example.com", Password = "newpassword123", Role = "Admin" };
            _mockUserService.Setup(service => service.UpdateUserAsync(userId, userRequest)).ThrowsAsync(new Exception("User not found."));

            // Act
            var result = await _usersController.UpdateUser(userId, userRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User not found.", notFoundResult.Value);
        }

        [Fact]
        public async Task DeleteUser_ExistingId_ReturnsNoContent()
        {
            // Arrange
            var userId = 1;

            // Act
            var result = await _usersController.DeleteUser(userId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockUserService.Verify(service => service.DeleteUserAsync(userId), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var userId = 1;
            _mockUserService.Setup(service => service.DeleteUserAsync(userId)).ThrowsAsync(new Exception("User not found."));

            // Act
            var result = await _usersController.DeleteUser(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User not found.", notFoundResult.Value);
        }
    }
}
