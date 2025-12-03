using AutoMapper;
using HealthStack.Auth.Api.DTOs;
using HealthStack.Auth.Api.Models;

namespace HealthStack.Auth.Api.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            // Domain -> Read User DTO
            CreateMap<User, UserReadDto>();

            // Create User DTO -> Domain
            CreateMap<UserRegisterDto, User>();

            // Login User DTO -> Domain
            CreateMap<UserLoginDto, User>();
        }
    }
}