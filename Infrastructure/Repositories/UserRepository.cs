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
        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<User> GetUserById(int id)
        {
            return await _context.Users
                .Include(u => u.Events)
                .FirstOrDefaultAsync(u => u.Id == id);
        }
        public async Task<User> GetUserByName(string name)
        {
            return await _context.Users
                .Include(u => u.Events)
                .FirstOrDefaultAsync(u => u.Name == name);
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> UserExistsByNameAsync(string name)
        {
            return await _context.Users.AnyAsync(u => u.Name == name);

        }
    }
}
