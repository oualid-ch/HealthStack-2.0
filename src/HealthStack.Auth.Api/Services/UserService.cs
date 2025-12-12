using HealthStack.Auth.Api.Data;
using HealthStack.Auth.Api.Exceptions;
using HealthStack.Auth.Api.Models;
using HealthStack.Auth.Api.Utils;
using Microsoft.EntityFrameworkCore;

namespace HealthStack.Auth.Api.Services
{
public class UserService(AppDbContext context, TokenProvider tokenProvider, ILogger<UserService> logger) : IUserService
    {
        private readonly AppDbContext _context = context;
        private readonly TokenProvider _tokenProvider = tokenProvider;
        private readonly ILogger<UserService> _logger = logger;

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            var user = await _context.Users
                .Include(u => u.Addresses)
                .SingleOrDefaultAsync(u => u.Id == id);
            
            return user ?? throw new UserNotFoundException("User not found");
        }

        public async Task<(User user, string token)> LoginUserAsync(string email, string password)
        {
            _logger.LogInformation("Login attempt for email: {Email}", LogUtils.MaskEmail(email));

            // search for user by provided email
            var user = await _context.Users
                .Include(u => u.Addresses)
                .SingleOrDefaultAsync(u => u.Email == email) ?? throw new UserNotFoundException(email);
            
            bool isValid = BCrypt.Net.BCrypt.Verify(password, user.Password);

            // verify password match
            if (!isValid)
                throw new InvalidPasswordException(user.Id);
            
            _logger.LogInformation("User {UserId} successfully logged in", LogUtils.MaskGuid(user.Id));
            return (user, _tokenProvider.GenerateToken(user));
        }

        public async Task<(User user, string token)> RegisterUserAsync(User user)
        {
            _logger.LogInformation("Registering new user with email {Email}", LogUtils.MaskEmail(user.Email));

            // check email availability
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                throw new EmailAlreadyExistsException(user.Email);

            // encrypt password
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            string token = _tokenProvider.GenerateToken(user);

            // save user to db
            _context.Users.Add(user);
                await _context.SaveChangesAsync();

            _logger.LogInformation("User {UserId} registered successfully", LogUtils.MaskGuid(user.Id));
            
            return (user, token);
        }
    }
}