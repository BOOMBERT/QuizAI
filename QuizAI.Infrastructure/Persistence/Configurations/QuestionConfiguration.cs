using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizAI.Domain.Entities;

namespace QuizAI.Infrastructure.Persistence.Configurations;

internal class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.Property(qn => qn.Content)
            .HasMaxLength(512);

        builder.HasOne(qn => qn.Image)
            .WithMany(i => i.Questions)
            .HasForeignKey(qn => qn.ImageId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(qn => qn.Quiz)
            .WithMany(qz => qz.Questions)
            .HasForeignKey(qn => qn.QuizId);
    }
}
