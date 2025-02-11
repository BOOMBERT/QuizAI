namespace QuizAI.Domain.Entities;

public class UserAnswer
{
    public Guid Id { get; set; }
    public Guid QuizAttemptId { get; set; }
    public QuizAttempt QuizAttempt { get; set; }
    public int QuestionId { get; set; }
    public Question Question { get; set; }
    public IList<string> AnswerText { get; set; } = new List<string>();
    public bool IsCorrect { get; set; }
    public DateTime AnsweredAt { get; set; }
}
