using System.Security.Claims;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using HealthStack.Auth.Api.Controllers;
using HealthStack.Auth.Api.DTOs;
using HealthStack.Auth.Api.Models;
using HealthStack.Auth.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HealthStack.Auth.UnitTests
{
    public class UserControllerTests
    {
        private readonly Mock<IValidator<UserLoginDto>> _loginValidatorMock = new();
        private readonly Mock<IValidator<UserRegisterDto>> _registerValidatorMock = new();
        private readonly Mock<IUserService> _userServiceMock = new();
        private readonly Mock<IMapper> _mapperMock = new();

        private UserController CreateController(ClaimsPrincipal? user = null)
        {
            var controller = new UserController(
                _loginValidatorMock.Object,
                _registerValidatorMock.Object,
                _userServiceMock.Object,
                _mapperMock.Object
            );

            if (user != null)
                controller.ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user }
                };

            return controller;
        }

        // -------------------------------------------------------------
        // LOGIN
        // -------------------------------------------------------------
        [Fact]
        public async Task LoginUser_ValidCredentials_ReturnsOkWithTokenAndUser()
        {
            // Arrange
            var loginDto = new UserLoginDto { Email = "test@test.com", Password = "Password123!" };
            _loginValidatorMock
                .Setup(v => v.Validate(loginDto))
                .Returns(new ValidationResult());

            var user = new User { Id = Guid.NewGuid(), Email = loginDto.Email };
            _userServiceMock
                .Setup(s => s.LoginUserAsync(loginDto.Email, loginDto.Password))
                .ReturnsAsync((user, "TEST_TOKEN"));

            var userReadDto = new UserReadDto { Id = user.Id, Email = user.Email };
            _mapperMock
                .Setup(m => m.Map<UserReadDto>(user))
                .Returns(userReadDto);

            var controller = CreateController();

            // Act
            var result = await controller.LoginUser(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var body = Assert.IsType<LoginResponseDto>(okResult.Value);
            
            body.Token.Should().Be("TEST_TOKEN");
            body.User.Should().BeEquivalentTo(userReadDto);

        }

        [Fact]
        public async Task LoginUser_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var loginDto = new UserLoginDto();
            _loginValidatorMock.Setup(v => v.Validate(loginDto))
                .Returns(new ValidationResult([new ValidationFailure("Email", "Required")]));

            var controller = CreateController();

            // Act
            var result = await controller.LoginUser(loginDto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var errors = Assert.IsType<Dictionary<string, string[]>>(badRequest.Value);

            errors.Should().ContainKey("Email");
            errors["Email"].Should().Contain("Required");

        }

        // -------------------------------------------------------------
        // REGISTER
        // -------------------------------------------------------------
        [Fact]
        public async Task RegisterUser_ValidModel_ReturnsCreatedWithTokenAndUser()
        {
            // Arrange
            var registerDto = new UserRegisterDto 
            { 
                Email = "test@test.com",
                FirstName = "FirstName",
                LastName = "LastName",
                Password = "Password123!"
            };
            _registerValidatorMock
                .Setup(v => v.Validate(registerDto))
                .Returns(new ValidationResult());

            var user = new User { Id = Guid.NewGuid(), Email = registerDto.Email };
            _mapperMock
                .Setup(m => m.Map<User>(registerDto))
                .Returns(user);

            _userServiceMock
                .Setup(s => s.RegisterUserAsync(user))
                .ReturnsAsync((user, "TEST_TOKEN"));

            var userReadDto = new UserReadDto { Id = user.Id, Email = user.Email };
            _mapperMock
                .Setup(m => m.Map<UserReadDto>(user))
                .Returns(userReadDto);

            var controller = CreateController();

            // Act
            var result = await controller.RegisterUser(registerDto);

            // Assert
            var ActionResult = Assert.IsType<ActionResult<UserReadDto>>(result);
            var createdResult = Assert.IsType<CreatedAtActionResult>(ActionResult.Result);
            var body = Assert.IsType<RegisterResponseDto>(createdResult.Value);

            body.Token.Should().Be("TEST_TOKEN");
            body.User.Should().BeEquivalentTo(userReadDto);
        }

        [Fact]
        public async Task RegisterUser_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var registerDto = new UserRegisterDto();
            _registerValidatorMock
                .Setup(v => v.Validate(registerDto))
                .Returns(new ValidationResult([new ValidationFailure("Email", "Required")]));

            var controller = CreateController();

            // Act
            var result = await controller.RegisterUser(registerDto);

            // Assert
            var ActionResult = Assert.IsType<ActionResult<UserReadDto>>(result);

            var badRequest = ActionResult.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            var errors = badRequest.Value.Should().BeOfType<Dictionary<string, string[]>>().Subject;
        }

        // -------------------------------------------------------------
        // GET CURRENT USER
        // -------------------------------------------------------------
        [Fact]
        public async Task GetCurrentUser_AuthenticatedUser_ReturnsOkWithUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            ]));

            var user = new User { Id = userId, Email = "test@test.com" };
            _userServiceMock.Setup(s => s.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            var userReadDto = new UserReadDto { Id = user.Id, Email = user.Email };
            _mapperMock.Setup(m => m.Map<UserReadDto>(user)).Returns(userReadDto);

            var controller = CreateController(claimsPrincipal);

            // Act
            var result = await controller.GetCurrentUser();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.Value.Should().BeEquivalentTo(userReadDto);
        }

        [Fact]
        public async Task GetCurrentUser_NoUserIdClaim_ReturnsUnauthorized()
        {
            // Arrange
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
            var controller = CreateController(claimsPrincipal);

            // Act
            var result = await controller.GetCurrentUser();

            // Assert
            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            var body = Assert.IsType<ErrorResponseDto>(unauthorized.Value);

            body.Message.Should().Be("Authentication required");
        }
    }
}
