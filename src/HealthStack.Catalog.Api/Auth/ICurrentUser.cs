namespace HealthStack.Catalog.Api.Auth;
public interface ICurrentUser
{
    Guid? UserId { get; }
    string? Role { get; }
    string? Email { get; }
}
