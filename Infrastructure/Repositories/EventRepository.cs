using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDbContext _context;
        public EventRepository(ApplicationDbContext context) 
        {
            _context = context;
        }

        public async Task AddAsync(Event eventModel)
        {
            _context.Events.Add(eventModel);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var eventModel = await _context.Events.FindAsync(id);
            if (eventModel != null)
            {
                _context.Events.Remove(eventModel);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Event>> GetAllEvents()
        {
            return await _context.Events
                .ToListAsync();
        }

        public async Task<Event> GetAllUsers(int id)
        {
            return await _context.Events
                .Include(x => x.Users)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Event> GetEventById(int id)
        {
            return await _context.Events
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Event> GetEventByName(string name)
        {
            return await _context.Events
                .FirstOrDefaultAsync(x => x.Title == name);
        }

        public async Task RegisterUserToEventAsync(int eventId, int userId)
        {
            var eventModel = await _context.Events
                .Include(e => e.Users)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            var user = await _context.Users.FindAsync(userId);

            if (eventModel != null && user != null)
            {
                if (eventModel.Users == null)
                    eventModel.Users = new List<User>();

                if (!eventModel.Users.Any(u => u.Id == userId))
                {
                    eventModel.Users.Add(user);
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task UnregisterUserFromEventAsync(int eventId, int userId)
        {
            var eventModel = await _context.Events
                .Include(e => e.Users)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (eventModel != null && eventModel.Users != null)
            {
                var user = eventModel.Users.FirstOrDefault(u => u.Id == userId);
                if (user != null)
                {
                    eventModel.Users.Remove(user);
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task UpdateAsync(Event eventModel)
        {
            _context.Events.Update(eventModel);
            await _context.SaveChangesAsync();
        }
    }
}
