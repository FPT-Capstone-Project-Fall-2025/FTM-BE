using FTM.Domain.Entities.Identity;
using FTM.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using System.Text;

namespace FTM.API.Extensions
{
    public static class AddAuthenticationConfiguration
    {
        public static void AddAuthenConfig(this IServiceCollection serrvices)
        {
            serrvices.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = true;
                options.Password.RequiredUniqueChars = 1;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireLowercase = true;

                options.SignIn.RequireConfirmedEmail = true;
                options.Lockout.MaxFailedAccessAttempts = 6;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
            }).AddEntityFrameworkStores<AppIdentityDbContext>().AddDefaultTokenProviders();

            serrvices.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";
            })
            .AddJwtBearer("JwtBearer", options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
                    ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SIGNING_KEY")!)
                    )
                };

                // Allow Signal R read JWT Token
                options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/notification"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };

            }).AddGoogle(options =>
            {
                options.ClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENTID")!;
                options.ClientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENTSECRET")!;
            });

            // Add services to the container.
            serrvices.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                // Set token lifetime to 5 minutes
                options.TokenLifespan = TimeSpan.FromMinutes(5);
            });

            serrvices.AddCors(options =>
            {
                options.AddPolicy("AllowPorts", policy =>
                {
                    policy.WithOrigins(Environment.GetEnvironmentVariable("FE_URL")?? "http://localhost:8900", "http://localhost:5500")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

        }
    }
}
