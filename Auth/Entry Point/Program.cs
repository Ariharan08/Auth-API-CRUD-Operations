using Auth.Models;
using Auth.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configuration is automatically loaded from appsettings.json by default, no need to add it manually
// Log configuration values for debugging
Console.WriteLine($"SecretKey: {builder.Configuration["JwtSettings:SecretKey"]}");
Console.WriteLine($"ExpirationInMinutes: {builder.Configuration["JwtSettings:ExpirationInMinutes"]}");

var key = "defaultsecretkey123450000000000000000000000000000000000000000000000000";
// Add services to the container
builder.Services.AddSingleton(new JwtSettings
{
    SecretKey = builder.Configuration["JwtSettings:SecretKey"] ?? key, // Default value if null
    ExpirationInMinutes = int.Parse(builder.Configuration["JwtSettings:ExpirationInMinutes"] ?? "30") // Default value if null
});

// Register token service
builder.Services.AddScoped<ITokenService, TokenService>();

// Add controllers and Swagger for API documentation
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add authentication and configure JWT Bearer token validation
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"] ?? key)) // Default value if null
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Map the controllers for API endpoints
app.MapControllers();

// Run the application
app.Run();
