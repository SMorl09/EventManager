using Application.DTO.Request;
using Application.DTO.Response;

namespace Application.Interface
{
    public interface IEventService
    {
        Task<EventWithAddressResponse> CreateEventAsync(EventRequest eventRequest, string imageUrl);
        Task DeleteEventAsync(int id);
        Task<IEnumerable<EventResponse>> GetAllEvents();
        Task<IEnumerable<UserResponse>> GetAllUsers(int id);
        Task<EventResponse> GetEventByIdAsync(int id);
        Task<EventResponse> GetEventByNameAsync(string name);
        Task RegisterUserToEventAsync(int eventId, int userId);
        Task UnregisterUserFromEventAsync(int eventId, int userId);
        Task UpdateEventAsync(int id, EventRequest eventRequest);
        Task UpdateEventImageAsync(int id, string imageUrl);
        Task<EventResponse> CreateEventAsync(EventRequest eventRequest);
        Task<EventWithAddressResponse> CreateEventWithAddressAsync(EventRequest eventRequest);
        Task<EventWithAddressResponse> GetEventWithAddressByIdAsync(int id);
    }
}