using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizAI.Domain.Entities;

namespace QuizAI.Infrastructure.Persistence.Configurations;

internal class AnswerConfiguration : IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> builder)
    {
        builder.Property(a => a.Content)
            .HasMaxLength(255);

        builder.HasOne(a => a.Question)
            .WithMany(qn => qn.Answers)
            .HasForeignKey(a => a.QuestionId);
    }
}
