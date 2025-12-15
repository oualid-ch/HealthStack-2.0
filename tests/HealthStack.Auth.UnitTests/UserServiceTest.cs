using FluentAssertions;
using HealthStack.Auth.Api.Data;
using HealthStack.Auth.Api.Exceptions;
using HealthStack.Auth.Api.Models;
using HealthStack.Auth.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace HealthStack.Auth.UnitTests
{
    public class UserServiceTest
    {
        private readonly AppDbContext _context;
        private readonly Mock<ITokenProvider> _tokenProviderMock;
        private readonly Mock<ILogger<UserService>> _loggerMock;
        private readonly UserService _sut;

        public UserServiceTest()
        {
            // InMemory DB
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Unique DB per test
                .Options;

            _context = new AppDbContext(options);

            // Mocks
            _tokenProviderMock = new Mock<ITokenProvider>();
            _tokenProviderMock.Setup(tp => tp.GenerateToken(It.IsAny<User>()))
                .Returns("TEST_TOKEN");

            _loggerMock = new Mock<ILogger<UserService>>();

            // SUT
            _sut = new UserService(_context, _tokenProviderMock.Object, _loggerMock.Object);
        }

        // Helper to seed user
        private async Task<User> SeedUserAsync(string email = "test@test.com", string password = "Password123!")
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                FirstName = "First",
                LastName = "Last",
                Role = "User",
                Password = BCrypt.Net.BCrypt.HashPassword(password)
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        // -------------------------------------GetUserByIdAsync-------------------------------------
        [Fact]
        public async Task GetUserByIdAsync_UserExists_ReturnsUser()
        {
            // Arrange
            var seededUser = await SeedUserAsync();

            // Act
            var user = await _sut.GetUserByIdAsync(seededUser.Id);

            // Assert
            user.Should().BeEquivalentTo(seededUser);
            var savedUser = await _context.Users.FindAsync(user.Id);
            savedUser.Should().NotBeNull();
            savedUser!.Email.Should().Be(user.Email);
        }

        [Fact]
        public async Task GetUserByIdAsync_UserDoesNotExist_ThrowsUserNotFoundException()
        {
            // Arrange
            var seededUser = await SeedUserAsync();

            // Act
            Func<Task> act = async () => await _sut.GetUserByIdAsync(Guid.Parse("e02fd0e4-00fd-090a-ca30-0d00a0038ba0")); // Fake Guid

            // Assert
            await act.Should().ThrowAsync<UserIdNotFoundException>()
                .Where(e => e.UserId == Guid.Parse("e02fd0e4-00fd-090a-ca30-0d00a0038ba0"));
        }

        // --------------------------------------LoginUserAsync--------------------------------------
        [Fact]
        public async Task LoginUserAsync_ValidCredentials_ReturnsUserAndToken()
        {
            // Arrange
            var seededUser = await SeedUserAsync();

            // Act
            var (user, token) = await _sut.LoginUserAsync(seededUser.Email, "Password123!");

            // Assert
            token.Should().Be("TEST_TOKEN");
            user.Email.Should().Be(seededUser.Email);
        }

        [Fact]
        public async Task LoginUserAsync_UserDoesNotExist_ThrowsUserNotFoundException()
        {
            // Arrange
            var seededUser = await SeedUserAsync();

            // Act
            Func<Task> act = async () => await _sut.LoginUserAsync("wrong@mail.com", "password_does_not_matter");

            // Assert
            await act.Should().ThrowAsync<UserNotFoundException>()
                    .Where(e => e.Email == "wrong@mail.com");
        }
        
        [Fact]
        public async Task LoginUserAsync_InvalidPassword_ThrowsInvalidPasswordException()
        {
            // Arrange
            var seededUser = await SeedUserAsync();
            var WrongPassword = "WrongPassword!";

            // Act
            Func<Task> act = async () => await _sut.LoginUserAsync(seededUser.Email, WrongPassword);

            // Assert
            await act.Should().ThrowAsync<InvalidPasswordException>()
                    .Where(e => e.UserId == seededUser.Id);

            seededUser.Password.Should().NotBe(WrongPassword);
            BCrypt.Net.BCrypt.Verify("Password123!", seededUser.Password).Should().BeTrue();
        }
    
        // -------------------------------------RegisterUserAsync------------------------------------
        [Fact]
        public async Task RegisterUserAsync_EmailAvailable_CreatesUserAndReturnsToken()
        {
            // Arrange
            // await SeedUserAsync();

            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Email = "new@test.com",
                FirstName = "firstName",
                LastName = "lastName",
                Role = "User",
                Password = "Password123!"
            };

            // Act
            var (savedUser, token) = await _sut.RegisterUserAsync(newUser);

            // Assert
            token.Should().Be("TEST_TOKEN");
            savedUser.Email.Should().Be(newUser.Email);
        }

        [Fact]
        public async Task RegisterUserAsync_EmailExists_ThrowsEmailAlreadyExistsException()
        {
            // Arrange
            var seededUser = await SeedUserAsync();

            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@test.com",
                FirstName = "firstName",
                LastName = "lastName",
                Role = "User",
                Password = "Password123!"
            };

            // Act
            Func<Task> act = async () => await _sut.RegisterUserAsync(newUser);

            // Assert
            await act.Should().ThrowAsync<EmailAlreadyExistsException>()
                    .Where(e => e.Email == newUser.Email);
        }
    }
}