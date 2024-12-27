using Microsoft.EntityFrameworkCore;
using QuizAI.Domain.Entities;
using QuizAI.Infrastructure.Persistence.Configurations;

namespace QuizAI.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    internal DbSet<Quiz> Quizzes { get; set; }
    internal DbSet<Question> Questions { get; set; }
    internal DbSet<MultipleChoiceAnswer> MultipleChoiceAnswers { get; set; }
    internal DbSet<OpenEndedAnswer> OpenEndedAnswers { get; set; }
    internal DbSet<TrueFalseAnswer> TrueFalseAnswers { get; set; }
    internal DbSet<Category> Categories { get; set; }
    internal DbSet<Image> Images { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new QuizConfiguration());
        modelBuilder.ApplyConfiguration(new QuestionConfiguration());
        modelBuilder.ApplyConfiguration(new MultipleChoiceAnswerConfiguration());
        modelBuilder.ApplyConfiguration(new OpenEndedAnswerConfiguration());
        modelBuilder.ApplyConfiguration(new TrueFalseAnswerConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new ImageConfiguration());
    }
}
