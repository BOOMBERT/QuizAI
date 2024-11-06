using Microsoft.EntityFrameworkCore;
using QuizAI.Domain.Entities;
using QuizAI.Infrastructure.Persistence.Configurations;

namespace QuizAI.Infrastructure.Persistence;

internal class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    internal DbSet<Quiz> Quizzes { get; set; }
    internal DbSet<Question> Questions { get; set; }
    internal DbSet<Answer> Answers { get; set; }
    internal DbSet<Category> Categories { get; set; }
    internal DbSet<Image> Images { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new QuizConfiguration());
        modelBuilder.ApplyConfiguration(new QuestionConfiguration());
        modelBuilder.ApplyConfiguration(new AnswerConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new ImageConfiguration());
    }
}
