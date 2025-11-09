using FTM.Domain.Entities.Posts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FTM.Infrastructure.Configurations
{
    public class PostReactionConfiguration : IEntityTypeConfiguration<PostReaction>
    {
        public void Configure(EntityTypeBuilder<PostReaction> builder)
        {
            builder.ToTable("PostReactions");
            builder.HasKey(pr => pr.Id);

            builder.Property(pr => pr.ReactionType)
                .IsRequired();

            builder.Property(pr => pr.CreatedOn)
                .IsRequired();

            // Relationships
            builder.HasOne(pr => pr.Post)
                .WithMany(p => p.PostReactions)
                .HasForeignKey(pr => pr.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pr => pr.FTMember)
                .WithMany()
                .HasForeignKey(pr => pr.FTMemberId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(pr => pr.PostId);
            builder.HasIndex(pr => pr.FTMemberId);
            builder.HasIndex(pr => new { pr.PostId, pr.FTMemberId })
                .IsUnique();
        }
    }
}
