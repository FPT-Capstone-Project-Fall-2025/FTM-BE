using FTM.API.Extensions;
using FTM.Infrastructure.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddIdentityAppDbContext();
builder.Services.AddFTMDbContext();
builder.Services.AddDI();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

//app.UseAuthorization();

// <-----------------------Custom Middleware------------------------------------->
app.UseLoggerMiddleware();
// <-----------------------End Custom Middleware------------------------------------->

// Map Health Check endpoint
app.MapHealthChecks("/health");
app.MapControllers();

app.Run();
