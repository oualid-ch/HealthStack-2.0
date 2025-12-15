using System.Net.Http.Json;
using FluentAssertions;
using HealthStack.Auth.Api.DTOs;

namespace HealthStack.Auth.IntegrationTests;

public class UserLoginTests(AuthWebApplicationFactory factory) : IntegrationTestBase(factory), IClassFixture<AuthWebApplicationFactory>
{
    [Fact]
    public async Task LoginUser_ReturnsTokenAndUserAsync()
    {
        // Arrange

        // Register user first
        await Client.PostAsJsonAsync("/api/user/register",
        new UserRegisterDto
        {
            Email = "login.integration.test@example.com",
            Password = "Pass@word123",
            FirstName = "Jane",
            LastName = "Doey"
        });
        
        // Act
        var response = await Client.PostAsJsonAsync(
            "/api/user/login",
            new UserLoginDto
            {
                Email= "login.integration.test@example.com",
                Password = "Pass@word123"
            }
        );

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        
        body.Should().NotBeNull();
        body.Token.Should().NotBeNullOrEmpty();
    }
}
