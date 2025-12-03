using FluentValidation;
using HealthStack.Auth.Api.Data;
using HealthStack.Auth.Api.Services;
using HealthStack.Auth.Api.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();

// Add services to the container.

// var hashedPassword = BCrypt.Net.BCrypt.HashPassword("password123");
// Console.WriteLine(hashedPassword);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>
    (options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
