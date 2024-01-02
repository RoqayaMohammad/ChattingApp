using ChattingApp.Interfaces;
using ChattingApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using static System.Net.Mime.MediaTypeNames;

namespace ChattingApp.Data
{
    public class UserRepository : IUSerRepositorye
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
                
        }
        public async Task<IEnumerable<AppUser>> GetUserByIdAsync(int id)
        {
            return await _context.AppUsers.FindAsync(id);
        }

        public async Task<AppUser> GetUSerByUsernameAsync(string username)
        {
            return await _context.AppUsers.ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update()
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}
