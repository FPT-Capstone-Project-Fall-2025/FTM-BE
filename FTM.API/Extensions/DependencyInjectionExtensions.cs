using FTM.Application.IServices;
using FTM.Application.Services;
using FTM.Domain.Entities.Identity;
using FTM.Infrastructure.Data;
using FTM.Infrastructure.Repositories.Implement;
using FTM.Infrastructure.Repositories.Interface;
using Microsoft.AspNetCore.Identity;

namespace FTM.API.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static void AddDI(this IServiceCollection serrvices) {
            //Services
            serrvices.AddScoped<ITokenProvider, TokenProvider>();
            serrvices.AddScoped<IAccountService, AccountService>();
            serrvices.AddScoped<ICurrentUserResolver, CurrentUserResolver>();
            serrvices.AddScoped<ISendSMSService, SendSMSService>();
            serrvices.AddScoped<IBlobStorageService, BlobStorageService>();
            //Repositories
            serrvices.AddScoped<ISendOTPTrackingRepository, SendOTPTrackingRepository>();
            serrvices.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
