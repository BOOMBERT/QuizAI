using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizAI.Domain.Entities;

namespace QuizAI.Infrastructure.Persistence.Configurations;

internal class ImageConfiguration : IEntityTypeConfiguration<Image>
{
    public void Configure(EntityTypeBuilder<Image> builder)
    {
        builder.Property(i => i.FilePath)
            .HasMaxLength(255);

        builder.Property(i => i.Name)
            .HasMaxLength(128);

        builder.Property(i => i.FileExtension)
            .HasMaxLength(4);

        builder.Property(i => i.Hash)
            .HasMaxLength(64);
        
        builder.HasIndex(i => i.Hash)
            .IsUnique();
    }
}
