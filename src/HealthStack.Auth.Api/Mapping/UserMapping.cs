// using HealthStack.Auth.Api.DTOs;
// using HealthStack.Auth.Api.Models;
/*


--------MANUAL MAPPING--------


*/
// namespace HealthStack.Auth.Api.Mapping
// {
//     public static class UserMapping
//     {
//         // Entity -> Read User DTO
//         public static UserReadDto ToDto(this User user)
//         {
//             return new UserReadDto
//             {
//                 Id = user.Id,
//                 FirstName = user.FirstName,
//                 LastName = user.LastName,
//                 Email = user.Email,
//                 Role = user.Role,
//                 DateOfBirth = user.DateOfBirth,
//                 PhoneNumber = user.PhoneNumber,
//                 Addresses = user.Addresses,
//                 CreatedAt = user.CreatedAt,
//                 UpdatedAt = user.UpdatedAt
//             };
//         }

//         // Create User DTO -> Entity
//         public static User ToEntity(this UserRegisterDto UserReadDto)
//         {
//             return new User()
//             {
//                 FirstName = UserReadDto.FirstName,
//                 LastName = UserReadDto.LastName,
//                 Email = UserReadDto.Email,
//                 Password = UserReadDto.Password
//             };
//         }

//         // Login User DTO -> Entity
//         public static void ToEntity(this UserLoginDto loginDto, User user)
//         {
//             user.Email = loginDto.Email;
//             user.Password = loginDto.Password;
//         }
//     }
// }