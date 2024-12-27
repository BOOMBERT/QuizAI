namespace QuizAI.Domain.Entities;

public class TrueFalseAnswer
{
    public int Id { get; set; }
    public bool IsCorrect { get; set; }

    public int QuestionId { get; set; }
    public Question Question { get; set; }
}
