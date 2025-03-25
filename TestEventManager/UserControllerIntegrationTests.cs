using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using EventManager;
using Application.DTO.Request;
using Domain.Data;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Application.DTO.Response;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;

namespace TestEventManager
{
    public class UserControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly string _token;

        public UserControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");

                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("InMemoryDbForTesting");
                    });

                    var sp = services.BuildServiceProvider();
                    using var scope = sp.CreateScope();
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<ApplicationDbContext>();

                    db.Database.EnsureCreated();

                    if (!db.Users.Any())
                    {
                        db.Users.Add(new User
                        {
                            Name = "John",
                            PasswordHash = "hashedPassword",
                            Role = RoleCategory.User,
                            Surename = "Doe",
                            BirthDate = "2000-01-01",
                            CreatedDate = DateTime.UtcNow.ToString("o"),
                            Email = "john.doe@example.com"
                        });
                        db.SaveChanges();
                    }
                });
            });

            _client = _factory.CreateClient();

            var jwtSettings = new
            {
                Key = "VerySecretKeyThatIsAtLeast32Chars",
                Issuer = "YourApp",
                Audience = "YourAppUsers"
            };
            _token = TestTokenHelper.GenerateJwtToken(jwtSettings.Key, jwtSettings.Issuer, jwtSettings.Audience, "1", "Admin");
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _token);
        }

        [Fact]
        public async Task GetUserById_ExistingUser_ReturnsOk()
        {
            var newUser = new UserRequest
            {
                Name = "John",
                Surename = "Doe",
                Email = "john.doe@example.com",
                Password = "password123",
                Role = "User",
                BirthDate = "2000-01-01"
            };

            // Act
            var postResponse = await _client.PostAsJsonAsync("/api/users", newUser);
            postResponse.EnsureSuccessStatusCode();
            var createdUser = await postResponse.Content.ReadFromJsonAsync<UserResponse>();

            // Act
            var getResponse = await _client.GetAsync($"/api/users/{createdUser.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            var userResponse = await getResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(userResponse);
            Assert.Equal(createdUser.Id, userResponse.Id);
        }

        [Fact]
        public async Task GetUserById_NonExistingUser_ReturnsNotFound()
        {
            // Act
            var response = await _client.GetAsync("/api/users/9999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreateUser_ValidRequest_ReturnsCreatedUser()
        {
            // Arrange
            var newUser = new UserRequest
            {
                Name = "Jane",
                Password = "password123",
                Surename = "Doe",
                BirthDate = "1990-05-05",
                Email = "jane.doe@example.com",
                Role = "User"
            };

            var content = new StringContent(JsonConvert.SerializeObject(newUser), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/users", content);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var createdUser = await response.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(createdUser);
            Assert.Equal("Jane", createdUser.Name);
        }

        [Fact]
        public async Task UpdateUser_ExistingUser_ReturnsNoContent()
        {
            // Arrange
            var updatedUser = new UserRequest
            {
                Name = "John Updated",
                Password = "newpassword",
                Surename = "Doe Updated",
                BirthDate = "2000-01-01",
                Email = "john.updated@example.com",
                Role = "Admin"
            };

            // Act
            var response = await _client.PutAsJsonAsync("/api/users/1", updatedUser);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var getResponse = await _client.GetAsync("/api/users/1");
            getResponse.EnsureSuccessStatusCode();
            var userResponse = await getResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.Equal("John Updated", userResponse.Name);
        }

        [Fact]
        public async Task UpdateUser_NonExistingUser_ReturnsNotFound()
        {
            // Arrange
            var updatedUser = new UserRequest
            {
                Name = "Doesnt Matter",
                Password = "whatever",
                Surename = "NoUser",
                BirthDate = "2000-01-01",
                Email = "nouser@example.com",
                Role = "User"
            };

            // Act
            var response = await _client.PutAsJsonAsync("/api/users/9999", updatedUser);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteUser_ExistingUser_ReturnsNoContent()
        {
            // Act
            var response = await _client.DeleteAsync("/api/users/1");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var getResponse = await _client.GetAsync("/api/users/1");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task DeleteUser_NonExistingUser_ReturnsNotFound()
        {
            // Act
            var response = await _client.DeleteAsync("/api/users/9999");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}
