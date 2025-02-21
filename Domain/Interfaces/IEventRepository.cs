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
        Task<IEnumerable<Event>> GetAllEvents();
        Task<Event> GetEventById(int id);
        Task<Event> GetEventByName(string name);
        Task AddAsync(Event eventModel);
        Task UpdateAsync(Event eventModel);
        Task DeleteAsync(int id);
        Task RegisterUserToEventAsync(int eventId, int userId);
        Task<Event> GetAllUsers(int id);
        Task UnregisterUserFromEventAsync(int eventId, int userId);
    }
}
