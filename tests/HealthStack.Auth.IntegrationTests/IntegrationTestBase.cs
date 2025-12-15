namespace HealthStack.Auth.IntegrationTests;

public abstract class IntegrationTestBase(AuthWebApplicationFactory factory)
{
    protected readonly HttpClient Client = factory.CreateClient();
}
