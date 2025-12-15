using HealthStack.Auth.Api.Data;
using HealthStack.Auth.Api.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace HealthStack.Auth.IntegrationTests;

public class AuthWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        builder.ConfigureServices(services =>
        {
            // Remove existing DbContext registrations (from Program.cs)
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(AppDbContext));
            if (dbContextDescriptor != null)
                services.Remove(dbContextDescriptor);

            // Add InMemory Db for testing
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("AuthTestDb");
            });

            // Re-register any required services
            services.AddScoped<ITokenProvider, TokenProvider>();
            services.AddScoped<UserService>();
        });
    }
}
