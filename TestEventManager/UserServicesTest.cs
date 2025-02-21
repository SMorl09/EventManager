using Application.DTO.Request;
using Application.Services;
using Domain.Data;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEventManager
{
    public class UserServicesTest
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly UserService _userService;

        public UserServicesTest()
        {
            _mockUserRepository = new Mock<IUserRepository>();

            _userService = new UserService(_mockUserRepository.Object);
        }

        [Fact]
        public async Task GetUserByIdAsync_ExistingId_ReturnsUserResponse()
        {
            // Arrange
            var userId = 1;
            var user = new User { Id = userId, Name = "John", Surename = "Doe", BirthDate = "1990-01-01", CreatedDate = "2023-09-01", Email = "john.doe@example.com" };
            _mockUserRepository.Setup(repo => repo.GetUserById(userId)).ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
        }

        [Fact]
        public async Task CreateUserAsync_ValidRequest_ReturnsUserResponse()
        {
            // Arrange
            var userRequest = new UserRequest { Name = "Jane", Surename = "Doe", BirthDate = "1990-01-01", Email = "jane.doe@example.com", Password = "password123", Role = "User" };
            var user = new User { Id = 1, Name = userRequest.Name, Surename = userRequest.Surename, BirthDate = userRequest.BirthDate, CreatedDate = DateTime.UtcNow.ToString("yyyy-MM-dd"), Email = userRequest.Email };

            _mockUserRepository.Setup(repo => repo.AddAsync(It.IsAny<User>())).Callback<User>(u => u.Id = 1);

            // Act
            var result = await _userService.CreateUserAsync(userRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userRequest.Name, result.Name);
        }

        [Fact]
        public async Task UpdateUserAsync_ExistingId_UpdatesUser()
        {
            // Arrange
            var userId = 1;
            var user = new User { Id = userId, Name = "John", Surename = "Doe", BirthDate = "1990-01-01", CreatedDate = "2023-09-01", Email = "john.doe@example.com", Role = RoleCategory.User };
            var userRequest = new UserRequest { Name = "John Updated", Surename = "Doe Updated", BirthDate = "1990-01-02", Email = "john.updated@example.com", Role = "Admin" };
            _mockUserRepository.Setup(repo => repo.GetUserById(userId)).ReturnsAsync(user);

            // Act
            await _userService.UpdateUserAsync(userId, userRequest);

            // Assert
            _mockUserRepository.Verify(repo => repo.UpdateAsync(It.Is<User>(u => u.Name == userRequest.Name && u.Surename == userRequest.Surename && u.BirthDate == userRequest.BirthDate && u.Email == userRequest.Email && u.Role == RoleCategory.Admin)), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_ExistingId_DeletesUser()
        {
            // Arrange
            var userId = 1;

            // Act
            await _userService.DeleteUserAsync(userId);

            // Assert
            _mockUserRepository.Verify(repo => repo.DeleteAsync(userId), Times.Once);
        }

        [Fact]
        public async Task AuthenticateAsync_ValidCredentials_ReturnsUser()
        {
            // Arrange
            var username = "john.doe";
            var password = "password123";
            var user = new User { Id = 1, Name = "John", Surename = "Doe", BirthDate = "1990-01-01", CreatedDate = "2023-09-01", Email = "john.doe@example.com", PasswordHash = new PasswordHasher<User>().HashPassword(null, password) };
            _mockUserRepository.Setup(repo => repo.GetUserByName(username)).ReturnsAsync(user);

            // Act
            var result = await _userService.AuthenticateAsync(username, password);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
        }
    }
}
