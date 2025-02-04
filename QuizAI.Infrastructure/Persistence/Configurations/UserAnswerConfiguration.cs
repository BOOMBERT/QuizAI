using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizAI.Domain.Entities;

namespace QuizAI.Infrastructure.Persistence.Configurations;

internal class UserAnswerConfiguration : IEntityTypeConfiguration<UserAnswer>
{
    public void Configure(EntityTypeBuilder<UserAnswer> builder)
    {
        builder.HasOne(ua => ua.QuizAttempt)
            .WithMany(qa => qa.UserAnswers)
            .HasForeignKey(ua => ua.QuizAttemptId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(ua => ua.Question)
            .WithMany(qn => qn.UserAnswers)
            .HasForeignKey(ua => ua.QuestionId);

        builder.Property(ua => ua.AnswerText)
            .HasMaxLength(2063);

        builder.Property(ua => ua.AnsweredAt)
            .HasDefaultValueSql("GETUTCDATE()");
    }
}
