using System.Security.Claims;
using AutoMapper;
using FluentValidation;
using HealthStack.Auth.Api.DTOs;
using HealthStack.Auth.Api.Models;
using HealthStack.Auth.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthStack.Auth.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IValidator<UserLoginDto> userLoginValidator, IValidator<UserRegisterDto> userRegisterValidator, IUserService userService, IMapper mapper) : ControllerBase
    {
        private readonly IValidator<UserLoginDto> _userLoginValidator = userLoginValidator;
        private readonly IValidator<UserRegisterDto> _userRegisterValidator = userRegisterValidator;
        private readonly IUserService _userService = userService;
        private readonly IMapper _mapper = mapper;


        // -------------------------------------------------------------
        // GET USER BY ID
        // -------------------------------------------------------------
        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserReadDto>> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return user == null ? NotFound() : Ok(_mapper.Map<UserReadDto>(user));
        }

        // -------------------------------------------------------------
        // LOGIN
        // -------------------------------------------------------------
        [HttpPost("login")]
        public async Task<ActionResult> LoginUser(UserLoginDto userLoginDto)
        {
            var validation = _userLoginValidator.Validate(userLoginDto);
            if (!validation.IsValid)
                return BadRequest(validation.ToDictionary());

            var (user, token) = await _userService.LoginUserAsync(userLoginDto.Email, userLoginDto.Password);

            return user == null ? NotFound() : Ok(
                new
                {
                    Token = token,
                    User = _mapper.Map<UserReadDto>(user)
                }
            );
        }

        // -------------------------------------------------------------
        // REGISTER
        // -------------------------------------------------------------
        [HttpPost("register")]
        public async Task<ActionResult<UserReadDto>> RegisterUser(UserRegisterDto userRegisterDto)
        {
            var validation = _userRegisterValidator.Validate(userRegisterDto);
            if (!validation.IsValid)
                return BadRequest(validation.ToDictionary());
                
            // Check duplicate email
            if (await _userService.EmailExistsAsync(userRegisterDto.Email))
                return BadRequest(new { Email = new[] { "Email already registered" } });

            User user = _mapper.Map<User>(userRegisterDto);
            user.Role = "User";

            var (created, token) = await _userService.RegisterUserAsync(user);

            return CreatedAtAction(nameof(GetUserById), new { id = created.Id }, new
            {
                Token = token,
                User = _mapper.Map<UserReadDto>(created)
            });
        }

        // -------------------------------------------------------------
        // GET CURRENT USER (/me)
        // -------------------------------------------------------------
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var user = await _userService.GetUserByIdAsync(int.Parse(userId));
            if (user == null)
                return Unauthorized();

            return Ok(_mapper.Map<UserReadDto>(user));
        }

        [HttpGet("debug-token")]
        [Authorize]
        public IActionResult DebugToken()
        {
            return Ok(User.Claims.Select(c => new { c.Type, c.Value }));
        }
    }
}
