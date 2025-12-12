namespace HealthStack.Auth.Api.Exceptions
{
    public class InvalidPasswordException(Guid userId) : Exception($"Invalid password for user with ID {userId}")
    {
        public Guid UserId { get; } = userId;
    }
}