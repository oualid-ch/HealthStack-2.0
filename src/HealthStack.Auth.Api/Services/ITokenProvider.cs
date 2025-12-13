using HealthStack.Auth.Api.Models;

namespace HealthStack.Auth.Api.Services
{
    public interface ITokenProvider
    {
        public string GenerateToken(User user);
    }
}
