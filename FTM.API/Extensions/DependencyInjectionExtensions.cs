using FTM.Application.IServices;
using FTM.Application.Services;
using FTM.Domain.Entities.Identity;
using FTM.Infrastructure.Data;
using FTM.Infrastructure.Repositories.Implement;
using FTM.Infrastructure.Repositories.Interface;
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
            serrvices.AddScoped<ICurrentUserResolver, CurrentUserResolver>();
            serrvices.AddTransient<IEmailSender, EmailSender>();
            serrvices.AddSingleton(new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(Environment.GetEnvironmentVariable("GMAIL-USERNAME"), Environment.GetEnvironmentVariable("GMAIL-PASSWORD")),
                EnableSsl = true
            });
            //Repositories
            serrvices.AddScoped<ISendOTPTrackingRepository, SendOTPTrackingRepository>();
            serrvices.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
