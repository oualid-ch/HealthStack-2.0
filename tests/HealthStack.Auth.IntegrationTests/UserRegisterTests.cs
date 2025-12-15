using System.Net.Http.Json;
using FluentAssertions;
using HealthStack.Auth.Api.DTOs;

namespace HealthStack.Auth.IntegrationTests;

public class UserRegisterTests(AuthWebApplicationFactory factory) : IntegrationTestBase(factory), IClassFixture<AuthWebApplicationFactory>
{
    [Fact]
    public async Task RegisterUser_ReturnsTokenAndUserAsync()
    {
        // Arrange
        var request = new UserRegisterDto
        {
            Email = "integration.test@example.com",
            Password = "Password123!",
            FirstName = "Jane",
            LastName = "Doey"
        };

        // Act
        var response = await Client.PostAsJsonAsync(
            "/api/user/register",
            request
        );

        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine(content);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<RegisterResponseDto>();

        body.Should().NotBeNull();
        body.Token.Should().NotBeNull();
        body.User.Email.Should().Be(request.Email);
    }
}
