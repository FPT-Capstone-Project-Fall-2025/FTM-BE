using FTM.API.Extensions;
using FTM.Domain.Entities.Identity;
using FTM.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using System.Text;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddIdentityAppDbContext();
builder.Services.AddFTMDbContext();
builder.Services.AddAuthenConfig();
builder.Services.AddDI();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
    { 
        Title = "FTM API", 
        Version = "v1" 
    });
    
    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
    });
    
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Add HealthChecks for both DbContexts
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppIdentityDbContext>("IdentityDb")
    .AddDbContextCheck<FTMDbContext>("FTMDb");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
// <-----------------------Custom Middleware------------------------------------->
app.UseLoggerMiddleware();
// <-----------------------End Custom Middleware------------------------------------->

// Map Health Check endpoint
app.MapHealthChecks("/health");
app.MapControllers();

// Seed data on startup with better error handling
try
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Starting data seeding...");
    await app.Services.SeedDataAsync();
    logger.LogInformation("Data seeding completed!");
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Failed to seed data on startup");
    // Don't throw - let app continue running
}

app.Run();
