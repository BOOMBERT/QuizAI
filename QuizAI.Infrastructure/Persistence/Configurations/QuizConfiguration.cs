﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizAI.Domain.Entities;

namespace QuizAI.Infrastructure.Persistence.Configurations;

internal class QuizConfiguration : IEntityTypeConfiguration<Quiz>
{
    public void Configure(EntityTypeBuilder<Quiz> builder)
    {
        builder.Property(qz => qz.Name)
            .HasMaxLength(128);

        builder.Property(qz => qz.Description)
            .HasMaxLength(512);

        builder.Property(qz => qz.CreationDate)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(qz => qz.Creator)
            .WithMany(u => u.CreatedQuizzes)
            .HasForeignKey(qz => qz.CreatorId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(qz => qz.Image)
            .WithMany(i => i.Quizzes)
            .HasForeignKey(qz => qz.ImageId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(qz => qz.Categories)
            .WithMany(c => c.Quizzes)
            .UsingEntity(j => j.ToTable("QuizCategories"));
    }
}
