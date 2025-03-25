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

        public async Task AddAsync(Event eventModel, CancellationToken cancellationToken)
        {
            _context.Events.Add(eventModel);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var eventModel = await _context.Events.FindAsync(id, cancellationToken);
            if (eventModel != null)
            {
                _context.Events.Remove(eventModel);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<IEnumerable<Event>> GetAllEvents(CancellationToken cancellationToken)
        {
            return await _context.Events
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<Event?> GetAllUsers(int id, CancellationToken cancellationToken)
        {
            return await _context.Events
                .AsNoTracking()
                .Include(x => x.Users)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<Event?> GetEventById(int id, CancellationToken cancellationToken)
        {
            return await _context.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<Event?> GetEventByName(string name, CancellationToken cancellationToken)
        {
            return await _context.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Title == name, cancellationToken);
        }

        public async Task<Event?> GetEventWithAddress(int id, CancellationToken cancellationToken)
        {
            return await _context.Events
                .AsNoTracking()
                .Include(x => x.Address)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task RegisterUserToEventAsync(int eventId, int userId, CancellationToken cancellationToken)
        {
            var eventModel = await _context.Events
                .Include(e => e.Users)
                .FirstOrDefaultAsync(e => e.Id == eventId, cancellationToken);

            var user = await _context.Users.FindAsync(userId, cancellationToken);

            if (eventModel != null && user != null)
            {
                if (eventModel.Users == null)
                    eventModel.Users = new List<User>();

                if (!eventModel.Users.Any(u => u.Id == userId))
                {
                    eventModel.Users.Add(user);
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }
        }

        public async Task UnregisterUserFromEventAsync(int eventId, int userId, CancellationToken cancellationToken)
        {
            var eventModel = await _context.Events
                .Include(e => e.Users)
                .FirstOrDefaultAsync(e => e.Id == eventId, cancellationToken);

            if (eventModel != null && eventModel.Users != null)
            {
                var user = eventModel.Users.FirstOrDefault(u => u.Id == userId);
                if (user != null)
                {
                    eventModel.Users.Remove(user);
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }
        }

        public async Task UpdateAsync(Event eventModel, CancellationToken cancellationToken)
        {
            _context.Events.Update(eventModel);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
