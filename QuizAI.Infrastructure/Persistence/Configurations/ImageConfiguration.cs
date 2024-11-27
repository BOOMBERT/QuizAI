using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizAI.Domain.Entities;

namespace QuizAI.Infrastructure.Persistence.Configurations;

internal class ImageConfiguration : IEntityTypeConfiguration<Image>
{
    public void Configure(EntityTypeBuilder<Image> builder)
    {
        builder.Property(i => i.FileExtension)
            .HasMaxLength(5);

        builder.Property(i => i.Hash)
            .HasColumnType("VARBINARY(32)");

        builder.HasIndex(i => i.Hash)
            .IsUnique();
    }
}
