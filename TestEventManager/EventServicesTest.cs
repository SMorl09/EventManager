using Xunit;
using Moq;
using Application.Interface;
using Domain.Interfaces;
using Application.Services;
using Application.DTO.Request;
using Domain.Data;
using Domain.Models;
using Microsoft.AspNetCore.Hosting;
using System.Threading;

namespace TestEventManager
{
    public class EventServicesTest
    {
        private readonly Mock<IEventRepository> _mockEventRepository;
        private readonly Mock<IWebHostEnvironment> _mockEnv;
        private readonly IEventService _eventService;

        public EventServicesTest()
        {
            
            _mockEventRepository = new Mock<IEventRepository>();

            
            _mockEnv = new Mock<IWebHostEnvironment>();
            _mockEnv.Setup(env => env.WebRootPath).Returns(@"C:\\TestWebRoot");

            _eventService = new EventService(_mockEventRepository.Object, _mockEnv.Object);
        }

        [Fact]
        public async Task GetAllEvents_ReturnsListOfEventResponses()
        {
            // Arrange
            var events = new List<Event>
        {
            new Event { Id = 1, Title = "Event 1", Description = "Description 1", StartDate = "2024-01-01", Category = EventCategory.Conference, MaxNumberOfUsers = 100, ImageUrl = "image1.jpg" },
            new Event { Id = 2, Title = "Event 2", Description = "Description 2", StartDate = "2024-01-02", Category = EventCategory.Concert, MaxNumberOfUsers = 50, ImageUrl = "image2.jpg" }
        };
            _mockEventRepository.Setup(repo => repo.GetAllEvents(It.IsAny<CancellationToken>())).ReturnsAsync(events);

            var cancellationToken = CancellationToken.None;

            // Act
            var result = await _eventService.GetAllEvents(cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetEventByIdAsync_ExistingId_ReturnsEventResponse()
        {
            // Arrange
            var eventId = 1;
            var eventModel = new Event { Id = eventId, Title = "Event 1", Description = "Description 1", StartDate = "2024-01-02", Category = EventCategory.Concert, MaxNumberOfUsers = 100, ImageUrl = "image1.jpg" };
            _mockEventRepository.Setup(repo => repo.GetEventById(eventId, It.IsAny<CancellationToken>()))
             .ReturnsAsync(eventModel);

            var cancellationToken = CancellationToken.None;
            // Act
            var result = await _eventService.GetEventByIdAsync(eventId, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(eventId, result.Id);
        }

        [Fact]
        public async Task CreateEventAsync_ValidRequest_ReturnsEventResponse()
        {
            // Arrange
            var eventRequest = new EventRequest
            {
                Title = "New Event",
                Description = "New Description",
                StartDate = "2024-01-01",
                Category = "Conference",
                MaxNumberOfUsers = 150,
                Address = new AddressRequest { State = "Test", City = "Test", Street = "Test" }
            };

            
            _mockEventRepository.Setup(repo => repo.AddAsync(It.IsAny<Event>(), It.IsAny<CancellationToken>()))
           .Callback<Event, CancellationToken>((e, ct) => e.Id = 1)
           .Returns(Task.CompletedTask);

            var cancellationToken = CancellationToken.None;
            // Act
            var result = await _eventService.CreateEventAsync(eventRequest, "image1.jpg", cancellationToken);

            // Assert
            Assert.Equal(eventRequest.Title, result.Title);
            Assert.Equal("image1.jpg", result.ImageUrl);
        }

        [Fact]
        public async Task UpdateEventAsync_ExistingId_UpdatesEvent()
        {
            // Arrange
            var eventId = 1;
            var eventModel = new Event
            {
                Id = eventId,
                Title = "Event 1",
                Description = "Description 1",
                StartDate = "2024-01-01",
                Category = EventCategory.Conference,
                MaxNumberOfUsers = 100,
                ImageUrl = "image1.jpg"
            };
            var eventRequest = new EventRequest
            {
                Title = "Updated Event",
                Description = "Updated Description",
                StartDate = "2024-01-01",
                Category = "Party",
                MaxNumberOfUsers = 200
            };
            _mockEventRepository.Setup(repo => repo.GetEventById(eventId, It.IsAny<CancellationToken>()))
             .ReturnsAsync(eventModel);

            var cancellationToken = CancellationToken.None;
            // Act
            await _eventService.UpdateEventAsync(eventId, eventRequest, cancellationToken);

            // Assert
            _mockEventRepository.Verify(repo =>
            repo.UpdateAsync(It.Is<Event>(e =>
                e.Title == eventRequest.Title &&
                e.Description == eventRequest.Description &&
                e.StartDate == eventRequest.StartDate &&
                e.Category == EventCategory.Party &&
                e.MaxNumberOfUsers == eventRequest.MaxNumberOfUsers),
            It.IsAny<CancellationToken>()),
            Times.Once);
        }
    }
}