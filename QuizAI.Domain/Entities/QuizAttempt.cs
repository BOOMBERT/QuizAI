namespace QuizAI.Domain.Entities;

public class QuizAttempt
{
    public Guid Id { get; set; }
    public Guid QuizId { get; set; }
    public Quiz Quiz { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public int CurrentOrder { get; set; } = 1;
    public int CorrectAnswers { get; set; }

    public ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
}
