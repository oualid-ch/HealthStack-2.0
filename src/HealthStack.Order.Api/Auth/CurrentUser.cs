using System.Security.Claims;

namespace HealthStack.Order.Api.Auth;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public Guid? UserId =>
        Guid.TryParse(
            _httpContextAccessor.HttpContext?
                .User.FindFirstValue(ClaimTypes.NameIdentifier),
            out var id
        ) ? id : null;

    public string? Role =>
        _httpContextAccessor.HttpContext?
            .User.FindFirstValue(ClaimTypes.Role);

    public string? Email =>
        _httpContextAccessor.HttpContext?
            .User.FindFirstValue(ClaimTypes.Email);
}