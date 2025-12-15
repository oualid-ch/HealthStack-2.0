using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using HealthStack.Auth.Api.DTOs;

namespace HealthStack.Auth.IntegrationTests;

public class UserMeTests(AuthWebApplicationFactory factory) : IntegrationTestBase(factory), IClassFixture<AuthWebApplicationFactory>
{
    [Fact]
    public async Task GetMe_WithValidToken_ReturnsUser()
    {
        // Arrange
        
        // Register user first
        var registerResponse = await Client.PostAsJsonAsync(
            "/api/user/register",
            new UserRegisterDto
            {
                Email = "integration.test@example.com",
                Password = "Password123!",
                FirstName = "Jane",
                LastName = "Doey"
            });

        var registerBody = await registerResponse.Content.ReadFromJsonAsync<RegisterResponseDto>();
        
        Console.WriteLine(registerBody!.Token);

        // Add JWT
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", registerBody!.Token);
        
        // Act
        var response = await Client.GetAsync("/api/user/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetMe_WithoutToken_Returns401()
    {
        var response = await Client.GetAsync("/api/user/me");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
