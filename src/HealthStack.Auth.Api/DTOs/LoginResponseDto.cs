namespace HealthStack.Auth.Api.DTOs
{
    public record LoginResponseDto(string Token, UserReadDto User);
}
