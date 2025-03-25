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

namespace TestEventManager
{
    public class EventsControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly string _token;

        public EventsControllerIntegrationTests(WebApplicationFactory<Program> factory)
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
                    db.Events.AddRange(
                        new Event { Title = "Event 1", Description = "Description 1", StartDate = "2023-09-01", Category = EventCategory.Conference, MaxNumberOfUsers = 100, ImageUrl = "image1.jpg" },
                        new Event { Title = "Event 2", Description = "Description 2", StartDate = "2023-09-02", Category = EventCategory.Party, MaxNumberOfUsers = 50, ImageUrl = "image2.jpg" }
                    );
                    db.SaveChanges();
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
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
        }

        [Fact]
        public async Task GetAllEvents_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync("/api/events");

            // Assert
            response.EnsureSuccessStatusCode();
            var events = await response.Content.ReadAsStringAsync();
            Assert.NotNull(events);
        }

        [Fact]
        public async Task GetEventById_ReturnsOk_ForExistingEvent()
        {
            // Act
            var response = await _client.GetAsync("/api/events/1");

            // Assert
            response.EnsureSuccessStatusCode();
            var eventModel = await response.Content.ReadAsStringAsync();
            Assert.NotNull(eventModel);
        }

        [Fact]
        public async Task CreateEvent_ReturnsCreatedEvent()
        {
            // Arrange
            var newEvent = new EventRequest
            {
                Title = "New Event",
                Description = "New Description",
                StartDate = "2023-09-03",
                Category = "Conference",
                MaxNumberOfUsers = 150,
                Address = new AddressRequest
                {
                    State = "Test",
                    City = "Test",
                    Street = "Test"
                }
            };
            var content = new StringContent(JsonConvert.SerializeObject(newEvent), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/events", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var createdEvent = await response.Content.ReadAsStringAsync();
            Assert.NotNull(createdEvent);
        }
    }
}