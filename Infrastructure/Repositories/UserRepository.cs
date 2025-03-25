using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(User user, CancellationToken cancellationToken)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FindAsync(id, cancellationToken);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<User> GetUserById(int id, CancellationToken cancellationToken)
        {
            return await _context.Users
                .AsNoTracking()
                .Include(u => u.Events)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }
        public async Task<User> GetUserByName(string name, CancellationToken cancellationToken)
        {
            return await _context.Users
                .AsNoTracking()
                .Include(u => u.Events)
                .FirstOrDefaultAsync(u => u.Name == name, cancellationToken);
        }

        public async Task UpdateAsync(User user, CancellationToken cancellationToken)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync(cancellationToken);
        }
        public async Task<bool> UserExistsByNameAsync(string name, CancellationToken cancellationToken)
        {
            return await _context.Users.AnyAsync(u => u.Name == name, cancellationToken);

        }
    }
}
