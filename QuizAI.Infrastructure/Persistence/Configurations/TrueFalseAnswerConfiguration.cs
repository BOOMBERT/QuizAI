using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizAI.Domain.Entities;

namespace QuizAI.Infrastructure.Persistence.Configurations;

internal class TrueFalseAnswerConfiguration : IEntityTypeConfiguration<TrueFalseAnswer>
{
    public void Configure(EntityTypeBuilder<TrueFalseAnswer> builder)
    {
        builder.HasOne(tfa => tfa.Question)
            .WithMany(qn => qn.TrueFalseAnswers)
            .HasForeignKey(tfa => tfa.QuestionId);
    }
}
