using FTM.API.Extensions;
using FTM.Application.Services;
using FTM.Domain.Entities.Identity;
using FTM.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;
using System.Text;
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddIdentityAppDbContext();
builder.Services.AddFTMDbContext();
builder.Services.AddAuthenConfig();
builder.Services.AddDI();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerDocumentation();
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

app.Run();
