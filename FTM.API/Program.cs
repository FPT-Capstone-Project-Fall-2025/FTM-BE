using FTM.API.Extensions;
using FTM.Application.Hubs;
using FTM.Infrastructure.Configurations;
using FTM.Infrastructure.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddIdentityAppDbContext();
builder.Services.AddFTMDbContext();
builder.Services.AddAuthenConfig();
builder.Services.AddDI();
builder.Services.AddPayOSServices(builder.Configuration); // Add PayOS services
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        // Example: ignore circular references
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

        // Example: don�t preserve object references ($id/$ref will not appear)
        options.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None;

        // Example: use camelCase for property names
        options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();

        //Example: To make your API send and receive enum names like "View", "Edit", "Delete"
        options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });
builder.Services.AddOpenTelemetryConfig(builder.Environment);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddSwaggerGenNewtonsoftSupport();
// Add HealthChecks for both DbContexts
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppIdentityDbContext>("IdentityDb")
    .AddDbContextCheck<FTMDbContext>("FTMDb");

var app = builder.Build();
Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseLoggerMiddleware();
app.UseGlobalExceptionMiddleware();
app.UseCors("AllowPorts");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseFTAuthorizationMiddleware();

// Map Health Check endpoint
app.MapHealthChecks("/health");
app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notification");


// AUTO-MIGRATION
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var dbContexts = scope.ServiceProvider.GetServices<DbContext>();

    foreach (var db in dbContexts)
    {
        try
        {
            logger.LogInformation("Applying migrations for DbContext: {DbContext}", db.GetType().Name);
            db.Database.Migrate();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Migration failed for DbContext: {DbContext}", db.GetType().Name);
        }
    }
}

// SEEDING
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
}

app.Run();
