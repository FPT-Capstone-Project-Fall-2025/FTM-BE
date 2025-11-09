using FTM.Application.Hubs;
using FTM.Application.IServices;
using FTM.Application.Services;
using FTM.Domain.Entities.Identity;
using FTM.Infrastructure.Data;
using FTM.Infrastructure.Repositories;
using FTM.Infrastructure.Repositories.Implement;
using FTM.Infrastructure.Repositories.Interface;
using FTM.Infrastructure.Repositories.IRepositories;
using FTM.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.SignalR;
using System.Net;
using System.Net.Mail;

namespace FTM.API.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static void AddDI(this IServiceCollection serrvices) {
            //---------------------------Services----------------------------------------
            serrvices.AddScoped<ITokenProvider, TokenProvider>();
            serrvices.AddScoped<IAccountService, AccountService>();
            serrvices.AddScoped<ICurrentUserResolver, CurrentUserResolver>();
            serrvices.AddTransient<IEmailSender, EmailSender>();
            serrvices.AddTransient<IBlobStorageService, BlobStorageService>();
            serrvices.AddSingleton(new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(Environment.GetEnvironmentVariable("GMAIL_USERNAME"), Environment.GetEnvironmentVariable("GMAIL_PASSWORD")),
                EnableSsl = true
            });
            // Application Services
            serrvices.AddScoped<IEducationService, EducationService>();
            serrvices.AddScoped<IWorkService, WorkService>();

            // Bio
            serrvices.AddScoped<IBiographyService, BiographyService>();

            //Family Tree
            serrvices.AddScoped<IFamilyTreeService, FamilyTreeService>();
            serrvices.AddScoped<IFTMemberService, FTMemberService>();
            serrvices.AddScoped<IFTInvitationService, FTInvitationService>();

            // Notification ~ Signal R
            serrvices.AddScoped<IFTNotificationService, FTNotificationService>();

            // Auto Mapper
            serrvices.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            //FT Authorization
            serrvices.AddScoped<IFTAuthorizationService, FTAuthorizationService>();


            //-----------------Repositories-------------------------
            serrvices.AddScoped<IUnitOfWork, UnitOfWork>();
            serrvices.AddScoped<IUserRepository, UserRepository>();
            serrvices.AddScoped<IRoleRepository, RoleRepository>();

            // Bio
            serrvices.AddScoped<IBiographyRepository, BiographyRepository>();
            
            // Work & Education repositories
            serrvices.AddScoped<IEducationRepository, EducationRepository>();
            serrvices.AddScoped<IWorkRepository, WorkRepository>();

            //Family Tree
            serrvices.AddScoped<IFamilyTreeRepository, FamilyTreeRepository>();
            serrvices.AddScoped<IFTMemberRepository, FTMemberRepository>();
            serrvices.AddScoped<IFTRelationshipRepository, FTRelationshipRepository>();
            serrvices.AddScoped<IFTInvitationRepository, FTInvitationRepository>();
            serrvices.AddScoped<IFTUserRepository, FTUserRepository>();
            serrvices.AddScoped<IFTMemberFileRepository, FTMemberFileRepository>();

            // Notification ~ Signal R
            serrvices.AddScoped<IFTNotificationRepository, FTNotificationRepository>();
            serrvices.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

            // Application Services
            serrvices.AddScoped<IEducationService, EducationService>();
            serrvices.AddScoped<IWorkService, WorkService>();

            // Posts System
            serrvices.AddScoped<IPostRepository, PostRepository>();
            serrvices.AddScoped<IPostCommentRepository, PostCommentRepository>();
            serrvices.AddScoped<IPostReactionRepository, PostReactionRepository>();
            serrvices.AddScoped<IPostService, PostService>();
            
            // Honor Boards
            serrvices.AddScoped<IAcademicHonorRepository, AcademicHonorRepository>();
            serrvices.AddScoped<ICareerHonorRepository, CareerHonorRepository>();
            serrvices.AddScoped<IAcademicHonorService, AcademicHonorService>();
            serrvices.AddScoped<ICareerHonorService, CareerHonorService>();

            // Family Events
            serrvices.AddScoped<IFTFamilyEventRepository, FTFamilyEventRepository>();
            serrvices.AddScoped<IFTFamilyEventService, FTFamilyEventService>();

            // FT Authorization
            serrvices.AddScoped<IFTAuthorizationRepository, FTAuthorizationRepository>();

            // Fund Management
            // serrvices.AddScoped<IFTFundService, FTFundService>(); // TODO: Fix property mapping issues

            // Campaign Management
            serrvices.AddScoped<IFTCampaignService, FTCampaignService>();
            serrvices.AddScoped<IFTCampaignDonationService, FTCampaignDonationService>();
            serrvices.AddScoped<IFTCampaignExpenseService, FTCampaignExpenseService>();

            // PayOS Integration
            serrvices.AddScoped<IPayOSPaymentService, PayOSPaymentService>();

            // Generic 
            serrvices.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        }
    }
}
