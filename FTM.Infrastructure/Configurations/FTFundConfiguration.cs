using FTM.Domain.Entities.Funds;
using FTM.Domain.Entities.FamilyTree;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FTM.Infrastructure.Configurations
{
    public class FTFundConfiguration : IEntityTypeConfiguration<FTFund>
    {
        public void Configure(EntityTypeBuilder<FTFund> builder)
        {
            builder.ToTable("FTFunds")
                .HasKey(f => f.Id);

            builder.Property(f => f.FTId)
                .IsRequired();

            builder.Property(f => f.CurrentMoney)
                .IsRequired()
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            builder.Property(f => f.FundNote)
                .HasMaxLength(2000);

            builder.Property(f => f.FundName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(f => f.CreatedBy)
                .IsRequired();

            builder.Property(f => f.CreatedOn)
                .IsRequired();

            builder.Property(f => f.LastModifiedOn);
            
            builder.Property(f => f.LastModifiedBy)
                .HasMaxLength(200);

            builder.Property(f => f.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            // Relationships
            builder.HasOne(f => f.FamilyTree)
                .WithMany()
                .HasForeignKey(f => f.FTId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(f => f.Donations)
                .WithOne(d => d.Fund)
                .HasForeignKey(d => d.FTFundId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(f => f.Expenses)
                .WithOne(e => e.Fund)
                .HasForeignKey(e => e.FTFundId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(f => f.FTId);
            builder.HasIndex(f => f.IsDeleted);
            builder.HasIndex(f => f.CreatedOn);
        }
    }
}
