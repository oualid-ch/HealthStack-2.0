namespace HealthStack.Auth.Api.DTOs
{
    public record RegisterResponseDto(string Token, UserReadDto User);
}
