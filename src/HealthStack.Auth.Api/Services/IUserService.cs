using HealthStack.Auth.Api.DTOs;
using HealthStack.Auth.Api.Models;

namespace HealthStack.Auth.Api.Services
{
    public interface IUserService
    {
        public Task<User?> GetUserByIdAsync(int id);
        public Task<(User? user, string? token)> LoginUserAsync(string email, string password);
        public Task<(User user, string token)> RegisterUserAsync(User user);
        public Task<bool> EmailExistsAsync(string email);

        // TODO: Add templates for delete user, update user, update password, select all for admin???, admin routes?
    }
}
