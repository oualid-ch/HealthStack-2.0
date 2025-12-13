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
    public class UserController(
        IValidator<UserLoginDto> userLoginValidator, 
        IValidator<UserRegisterDto> userRegisterValidator, 
        IUserService userService, 
        IMapper mapper
        ) : ControllerBase
    {
        private readonly IValidator<UserLoginDto> _userLoginValidator = userLoginValidator;
        private readonly IValidator<UserRegisterDto> _userRegisterValidator = userRegisterValidator;
        private readonly IUserService _userService = userService;
        private readonly IMapper _mapper = mapper;


        // -------------------------------------------------------------
        // GET USER BY ID
        // Debug temporary endpoint!!
        // -------------------------------------------------------------
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<UserReadDto>> GetUserById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return Ok(_mapper.Map<UserReadDto>(user));
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

            return Ok(
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
            
            User user = _mapper.Map<User>(userRegisterDto);
            user.Role = "User";

            var (created, token) = await _userService.RegisterUserAsync(user);

            return CreatedAtAction(nameof(GetCurrentUser), null, new
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
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userIdClaim == null)
                return Unauthorized(new {message = "Authentication required"});

            var user = await _userService.GetUserByIdAsync(Guid.Parse(userIdClaim));
            return Ok(_mapper.Map<UserReadDto>(user));
        }
    }
}
