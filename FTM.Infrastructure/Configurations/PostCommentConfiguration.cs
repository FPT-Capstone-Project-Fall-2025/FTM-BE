using FTM.Domain.Entities.Posts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FTM.Infrastructure.Configurations
{
    public class PostCommentConfiguration : IEntityTypeConfiguration<PostComment>
    {
        public void Configure(EntityTypeBuilder<PostComment> builder)
        {
            builder.ToTable("PostComments");
            builder.HasKey(pc => pc.Id);

            builder.Property(pc => pc.Content)
                .IsRequired();

            builder.Property(pc => pc.ParentCommentId)
                .IsRequired(false); // Explicitly allow NULL for root comments

            builder.Property(pc => pc.CreatedOn)
                .IsRequired();

            // Relationships
            builder.HasOne(pc => pc.Post)
                .WithMany(p => p.PostComments)
                .HasForeignKey(pc => pc.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pc => pc.FTMember)
                .WithMany()
                .HasForeignKey(pc => pc.FTMemberId)
                .OnDelete(DeleteBehavior.Restrict);

            // Self-referencing relationship for nested comments
            builder.HasOne(pc => pc.ParentComment)
                .WithMany(pc => pc.ChildComments)
                .HasForeignKey(pc => pc.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(pc => pc.PostId);
            builder.HasIndex(pc => pc.FTMemberId);
            builder.HasIndex(pc => pc.ParentCommentId);
            builder.HasIndex(pc => pc.CreatedOn);
        }
    }
}
