using FTM.Application.IServices;
using FTM.Application.Services;
using FTM.Domain.Entities.Identity;
using FTM.Infrastructure.Data;
using FTM.Infrastructure.Repositories.Implement;
using FTM.Infrastructure.Repositories.Interface;
using FTM.Infrastructure.Repositories.IRepositories;
using FTM.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace FTM.API.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static void AddDI(this IServiceCollection serrvices) {
            //Services
            serrvices.AddScoped<ITokenProvider, TokenProvider>();
            serrvices.AddScoped<IAccountService, AccountService>();
            serrvices.AddScoped<IFamilyTreeService>(provider => 
                new FamilyTreeService(
                    provider.GetService<FTMDbContext>()!,
                    provider.GetService<AppIdentityDbContext>()!,
                    provider.GetService<ICurrentUserResolver>()!,
                    provider.GetService<IUnitOfWork>()!,
                    provider.GetService<IUserRepository>()!,
                    provider.GetService<IRoleRepository>()!,
                    provider.GetService<UserManager<ApplicationUser>>()!,
                    provider.GetService<RoleManager<ApplicationRole>>()!
                ));
            serrvices.AddScoped<ICurrentUserResolver, CurrentUserResolver>();
            serrvices.AddScoped<IBiographyService, BiographyService>();
            serrvices.AddTransient<IEmailSender, EmailSender>();
            serrvices.AddTransient<IBlobStorageService, BlobStorageService>();
            serrvices.AddSingleton(new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(Environment.GetEnvironmentVariable("GMAIL_USERNAME"), Environment.GetEnvironmentVariable("GMAIL_PASSWORD")),
                EnableSsl = true
            });
            //Repositories
            serrvices.AddScoped<IUnitOfWork, UnitOfWork>();
            serrvices.AddScoped<IUserRepository, UserRepository>();
            serrvices.AddScoped<IRoleRepository, RoleRepository>();
            serrvices.AddScoped<IBiographyRepository, BiographyRepository>();
            // Work & Education repositories
            serrvices.AddScoped<IEducationRepository, EducationRepository>();
            serrvices.AddScoped<IWorkRepository, WorkRepository>();

            // Application Services
            serrvices.AddScoped<IEducationService, EducationService>();
            serrvices.AddScoped<IWorkService, WorkService>();
        }
    }
}
