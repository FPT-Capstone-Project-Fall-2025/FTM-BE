using FTM.Domain.Entities.Posts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FTM.Infrastructure.Configurations
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.ToTable("Posts");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(p => p.Content)
                .IsRequired();

            builder.Property(p => p.Status)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(p => p.CreatedOn)
                .IsRequired();

            // Relationships
            builder.HasOne(p => p.FTMember)
                .WithMany()
                .HasForeignKey(p => p.FTMemberId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.PostComments)
                .WithOne(pc => pc.Post)
                .HasForeignKey(pc => pc.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.PostReactions)
                .WithOne(pr => pr.Post)
                .HasForeignKey(pr => pr.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.PostAttachments)
                .WithOne(pa => pa.Post)
                .HasForeignKey(pa => pa.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(p => p.FTMemberId);
            builder.HasIndex(p => p.Status);
            builder.HasIndex(p => p.CreatedOn);
        }
    }
}
