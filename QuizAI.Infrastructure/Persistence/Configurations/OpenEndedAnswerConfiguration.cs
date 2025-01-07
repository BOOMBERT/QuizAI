using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizAI.Domain.Entities;

namespace QuizAI.Infrastructure.Persistence.Configurations;

internal class OpenEndedAnswerConfiguration : IEntityTypeConfiguration<OpenEndedAnswer>
{
    public void Configure(EntityTypeBuilder<OpenEndedAnswer> builder)
    {
        builder.Property(oea => oea.ValidContent)
            .HasMaxLength(1291);

        builder.HasOne(oea => oea.Question)
            .WithOne(qn => qn.OpenEndedAnswer)
            .HasForeignKey<OpenEndedAnswer>(oea => oea.QuestionId);
    }
}
