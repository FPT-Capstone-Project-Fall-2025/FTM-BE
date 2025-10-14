using FTM.Domain.Entities.Posts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FTM.Infrastructure.Configurations
{
    public class PostAttachmentConfiguration : IEntityTypeConfiguration<PostAttachment>
    {
        public void Configure(EntityTypeBuilder<PostAttachment> builder)
        {
            builder.ToTable("PostAttachments");
            builder.HasKey(pa => pa.Id);

            builder.Property(pa => pa.FileUrl)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(pa => pa.FileType)
                .IsRequired();

            builder.Property(pa => pa.Caption)
                .HasMaxLength(500);

            builder.Property(pa => pa.CreatedOn)
                .IsRequired();

            // Relationships
            builder.HasOne(pa => pa.Post)
                .WithMany(p => p.PostAttachments)
                .HasForeignKey(pa => pa.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(pa => pa.PostId);
            builder.HasIndex(pa => pa.FileType);
        }
    }
}
