namespace HealthStack.Auth.Api.Exceptions
{
    public class UserIdNotFoundException(Guid userId) : Exception($"User not found for userId {userId}")
    {
        public Guid UserId { get; } = userId;
    }
}