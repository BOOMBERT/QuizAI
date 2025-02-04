using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizAI.Domain.Entities;

namespace QuizAI.Infrastructure.Persistence.Configurations;

internal class QuizAttemptConfiguration : IEntityTypeConfiguration<QuizAttempt>
{
    public void Configure(EntityTypeBuilder<QuizAttempt> builder)
    {
        builder.HasOne(qa => qa.Quiz)
            .WithMany(qz => qz.QuizAttempts)
            .HasForeignKey(qa => qa.QuizId);

        builder.HasOne(qa => qa.User)
            .WithMany(qz => qz.QuizAttempts)
            .HasForeignKey(qa => qa.UserId);

        builder.Property(qa => qa.StartedAt)
            .HasDefaultValueSql("GETUTCDATE()");         
    }
}
