using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizAI.Domain.Entities;

namespace QuizAI.Infrastructure.Persistence.Configurations;

public class QuizPermissionConfiguration : IEntityTypeConfiguration<QuizPermission>
{
    public void Configure(EntityTypeBuilder<QuizPermission> builder)
    {
        builder.HasOne(qp => qp.Quiz)
            .WithMany(qz => qz.QuizPermissions)
            .HasForeignKey(qp => qp.QuizId);

        builder.HasOne(qp => qp.User)
            .WithMany(u => u.QuizPermissions)
            .HasForeignKey(qp => qp.UserId);
    }
}
