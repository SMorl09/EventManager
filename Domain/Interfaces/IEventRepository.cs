using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IEventRepository
    {
        Task<IEnumerable<Event>> GetAllEvents(CancellationToken cancellationToken);
        Task<Event> GetEventById(int id, CancellationToken cancellationToken);
        Task<Event> GetEventByName(string name, CancellationToken cancellationToken);
        Task AddAsync(Event eventModel, CancellationToken cancellationToken);
        Task UpdateAsync(Event eventModel, CancellationToken cancellationToken);
        Task DeleteAsync(int id, CancellationToken cancellationToken);
        Task RegisterUserToEventAsync(int eventId, int userId, CancellationToken cancellationToken);
        Task<Event> GetAllUsers(int id, CancellationToken cancellationToken);
        Task UnregisterUserFromEventAsync(int eventId, int userId, CancellationToken cancellationToken);
        Task<Event> GetEventWithAddress(int id, CancellationToken cancellationToken);
    }
}
