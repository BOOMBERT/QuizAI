using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizAI.Domain.Entities;

namespace QuizAI.Infrastructure.Persistence.Configurations;

internal class TrueFalseAnswerConfiguration : IEntityTypeConfiguration<TrueFalseAnswer>
{
    public void Configure(EntityTypeBuilder<TrueFalseAnswer> builder)
    {
        builder.HasOne(tfa => tfa.Question)
            .WithOne(qn => qn.TrueFalseAnswer)
            .HasForeignKey<TrueFalseAnswer>(tfa => tfa.QuestionId);
    }
}
