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

                options.SignIn.RequireConfirmedPhoneNumber = true;
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
                    ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "FTM-API",
                    ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "FTM-Client",
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SIGNING_KEY") ?? "ThisIsMySecretKeyForFTMApplicationAndItShouldBeLongEnough123456789")
                    )
                };
            });
        }
    }
}
