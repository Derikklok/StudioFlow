using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using StudioFlow.Data;
using StudioFlow.Exceptions;
using StudioFlow.Repositories;
using StudioFlow.Repositories.Interfaces;
using StudioFlow.Services;
using StudioFlow.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers()
    .AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IProjectService, ProjectService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("Client", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // React dev server
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Configure logging to suppress verbose EF Core logs
builder.Logging
    .ClearProviders()
    .AddConsole()
    // Suppress all Entity Framework logs except critical errors
    .AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Critical)
    .AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Critical)
    .AddFilter("Microsoft.EntityFrameworkCore.Update", LogLevel.Critical)
    .AddFilter("Microsoft.EntityFrameworkCore.Infrastructure", LogLevel.Critical);


var app = builder.Build();

// Add global exception handler middleware
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (DuplicateEmailException ex)
    {
        // Handle duplicate email silently - no console logging
        context.Response.StatusCode = 400;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { error = ex.Message });
    }
    catch (UserAlreadyExistsException ex)
    {
        // Handle user already exists silently - no console logging
        context.Response.StatusCode = 400;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { error = ex.Message });
    }
    catch (InvalidCredentialsException ex)
    {
        // Handle invalid credentials silently - no console logging
        context.Response.StatusCode = 401;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { error = ex.Message });
    }
    catch (ProjectNotFoundException ex)
    {
        context.Response.StatusCode = 404;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(new
        {
            error = ex.Message
        });
    }
    catch (Exception ex)
    {
        // Log only unexpected errors with minimal info
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError("Unexpected error occurred: {Message}", ex.Message);

        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new
        {
            error = "An unexpected error occurred. Please try again later.",
            traceId = context.TraceIdentifier
        });
    }
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Enable CORS
app.UseCors("Client");

app.UseHttpsRedirection();

app.MapControllers();

app.Run();