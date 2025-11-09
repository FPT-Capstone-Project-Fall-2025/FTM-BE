using FTM.Domain.DTOs.Authen;
using FTM.Domain.Entities.Applications;
using FTM.Domain.Entities.Events;
using FTM.Domain.Entities.FamilyTree;
using FTM.Domain.Entities.Funds;
using FTM.Domain.Entities.Identity;
using FTM.Domain.Entities.Notifications;
using FTM.Domain.Entities.Posts;
using FTM.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Infrastructure.Data
{
    public class FTMDbContext : DbContext
    {
        public FTMDbContext()
        {
        }

        public FTMDbContext(DbContextOptions<FTMDbContext> options)
            : base(options)
        {
        }

        // Family Tree
        public virtual DbSet<Mprovince> Mprovinces { get; set; }
        public virtual DbSet<MWard> MWards { get; set; }
        public virtual DbSet<MEthnic> MEthnics { get; set; }
        public virtual DbSet<MReligion> MReligions { get; set; }
        public virtual DbSet<FTMember> FTMembers { get; set; }
        public virtual DbSet<FTMemberFile> FTMemberFiles { get; set; }
        public virtual DbSet<FamilyTree> FamilyTrees { get; set; }
        public virtual DbSet<FTRelationship> FTRelationships { get; set; }
        public virtual DbSet<FTAuthorization> FTAuthorizations { get; set; }
        public virtual DbSet<FTInvitation> FTInvitations { get; set; }
        public virtual DbSet<FTUser> FTUsers { get; set; }

        // Notifications
        public virtual DbSet<FTNotification> FTNotifications { get; set; }

        // Posts
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<PostComment> PostComments { get; set; }
        public virtual DbSet<PostReaction> PostReactions { get; set; }
        public virtual DbSet<PostAttachment> PostAttachments { get; set; }

        // Honor Boards
        public virtual DbSet<AcademicHonor> AcademicHonors { get; set; }
        public virtual DbSet<CareerHonor> CareerHonors { get; set; }

        // Family Events
        public virtual DbSet<FTFamilyEvent> FTFamilyEvents { get; set; }
        public virtual DbSet<FTFamilyEventMember> FTFamilyEventMembers { get; set; }
        public virtual DbSet<FTFamilyEventFT> FTFamilyEventFTs { get; set; }

        // Fund Management
        public virtual DbSet<FTFund> FTFunds { get; set; }
        public virtual DbSet<FTFundDonation> FTFundDonations { get; set; }
        public virtual DbSet<FTFundExpense> FTFundExpenses { get; set; }
        public virtual DbSet<FTFundCampaign> FTFundCampaigns { get; set; }
        public virtual DbSet<FTCampaignDonation> FTCampaignDonations { get; set; }
        public virtual DbSet<FTCampaignExpense> FTCampaignExpenses { get; set; }
        public virtual DbSet<FTPayOSTransaction> FTPayOSTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new MEthicConfiguration());
            builder.ApplyConfiguration(new MReligionConfiguration());
            builder.ApplyConfiguration(new MProvinceConfiguration());
            builder.ApplyConfiguration(new MWardConfiguration());
            builder.ApplyConfiguration(new FTMemberConfiguration());
            builder.ApplyConfiguration(new FTMemberFileConfiguration());
            builder.ApplyConfiguration(new FamilyTreeConfiguration());
            builder.ApplyConfiguration(new FTRelationshipConfiguration());
            builder.ApplyConfiguration(new FTAuthorizationConfiguration());
            builder.ApplyConfiguration(new FTInvitationConfiguration());
            builder.ApplyConfiguration(new FTNotificationConfiguration());
            builder.ApplyConfiguration(new FTUserConfiguration());
            builder.ApplyConfiguration(new PostConfiguration());
            builder.ApplyConfiguration(new PostCommentConfiguration());
            builder.ApplyConfiguration(new PostReactionConfiguration());
            builder.ApplyConfiguration(new PostAttachmentConfiguration());
            builder.ApplyConfiguration(new AcademicHonorConfiguration());
            builder.ApplyConfiguration(new CareerHonorConfiguration());
            builder.ApplyConfiguration(new FTFamilyEventConfiguration());
            builder.ApplyConfiguration(new FTFamilyEventMemberConfiguration());
            builder.ApplyConfiguration(new FTFamilyEventFTConfiguration());
            builder.ApplyConfiguration(new FTFundConfiguration());
            builder.ApplyConfiguration(new FTFundDonationConfiguration());
            builder.ApplyConfiguration(new FTFundExpenseConfiguration());
            builder.ApplyConfiguration(new FTFundCampaignConfiguration());
            builder.ApplyConfiguration(new FTCampaignDonationConfiguration());
            builder.ApplyConfiguration(new FTCampaignExpenseConfiguration());
            builder.ApplyConfiguration(new FTPayOSTransactionConfiguration());
            base.OnModelCreating(builder);
        }


    }
}
