namespace HealthStack.Auth.Api.Exceptions
{
    public class UserNotFoundException(string email) : Exception($"User not found for email {email}")
    {
        public string Email { get; } = email;
    }
}