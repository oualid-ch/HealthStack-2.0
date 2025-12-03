using HealthStack.Auth.Api.Data;
using HealthStack.Auth.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthStack.Auth.Api.Services
{
public class UserService(AppDbContext context) : IUserService
    {
        private readonly AppDbContext _context = context;

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Addresses)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> LoginUserAsync(string email, string password)
        {
            var user = await _context.Users
                .Include(u => u.Addresses)
                .FirstOrDefaultAsync(u => u.Email == email);
            
            if(user == null) return null;

            bool isValid = BCrypt.Net.BCrypt.Verify(password, user.Password);

             return isValid ? user : null;
        }

        public async Task<User> RegisterUserAsync(User user)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return user;
        }

        public async Task<User?> ValidateTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            const string prefix = "fake-token-";
            if (!token.StartsWith(prefix))
                return null;

            var span = token.AsSpan(prefix.Length);
            if (!int.TryParse(span, out int userId))
                return null;

            return await _context.Users
                .Include(u => u.Addresses)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
    
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

    }
}