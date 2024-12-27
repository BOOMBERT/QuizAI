using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizAI.Domain.Entities;

namespace QuizAI.Infrastructure.Persistence.Configurations;

internal class MultipleChoiceAnswerConfiguration : IEntityTypeConfiguration<MultipleChoiceAnswer>
{
    public void Configure(EntityTypeBuilder<MultipleChoiceAnswer> builder)
    {
        builder.Property(mca => mca.Content)
            .HasMaxLength(255);

        builder.HasOne(mca => mca.Question)
            .WithMany(qn => qn.MultipleChoiceAnswers)
            .HasForeignKey(mca => mca.QuestionId);
    }
}
