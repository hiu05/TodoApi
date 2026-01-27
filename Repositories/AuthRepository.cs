using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Entities;

namespace TodoApi.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly TodoDbContext _context;

        public AuthRepository(TodoDbContext context)
        {
            _context = context;
        }

        public async Task RegisterAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> LoginAsync(User user)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
        }
    }
}