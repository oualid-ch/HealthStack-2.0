namespace HealthStack.Auth.Api.Exceptions
{
    public class EmailAlreadyExistsException(string email) : Exception($"Email '{email}' already exists")
    {
        public string Email = email;
    }
}