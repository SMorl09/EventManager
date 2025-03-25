using Application.DTO.Request;
using Application.DTO.Response;
using Microsoft.AspNetCore.Http;

namespace Application.Interface
{
    public interface IEventService
    {
        Task<EventWithAddressResponse> CreateEventAsync(EventRequest eventRequest, string imageUrl, CancellationToken cancellationToken);
        Task DeleteEventAsync(int id, CancellationToken cancellationToken);
        Task<IEnumerable<EventResponse>> GetAllEvents(CancellationToken cancellationToken);
        Task<IEnumerable<UserResponse>> GetAllUsers(int id, CancellationToken cancellationToken);
        Task<EventResponse> GetEventByIdAsync(int id, CancellationToken cancellationToken);
        Task<EventResponse> GetEventByNameAsync(string name, CancellationToken cancellationToken);
        Task RegisterUserToEventAsync(int eventId, int userId, CancellationToken cancellationToken);
        Task UnregisterUserFromEventAsync(int eventId, int userId, CancellationToken cancellationToken);
        Task UpdateEventAsync(int id, EventRequest eventRequest, CancellationToken cancellationToken);
        Task UpdateEventImageAsync(int id, string imageUrl, CancellationToken cancellationToken);
        Task<EventResponse> CreateEventAsync(EventRequest eventRequest, CancellationToken cancellationToken);
        Task<EventWithAddressResponse> CreateEventWithAddressAsync(EventRequest eventRequest, CancellationToken cancellationToken);
        Task<EventWithAddressResponse> GetEventWithAddressByIdAsync(int id, CancellationToken cancellationToken);
        Task<string> SaveImageAsync(IFormFile image, HttpRequest request, CancellationToken cancellationToken);
    }
}