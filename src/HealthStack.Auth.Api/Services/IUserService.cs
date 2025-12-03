using HealthStack.Auth.Api.DTOs;
using HealthStack.Auth.Api.Models;

namespace HealthStack.Auth.Api.Services
{
    public interface IUserService
    {
        public Task<User?> GetUserByIdAsync(int id);
        public Task<User?> LoginUserAsync(string email, string password);
        public Task<User> RegisterUserAsync(User user);
        public Task<bool> EmailExistsAsync(string email);
        public Task<User?> ValidateTokenAsync(string token);

        // TODO: Add templates for delete user, update user, update password, select all for admin???, admin routes?
    }
}
