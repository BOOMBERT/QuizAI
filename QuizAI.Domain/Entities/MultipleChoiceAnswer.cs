namespace QuizAI.Domain.Entities;

public class MultipleChoiceAnswer
{
    public int Id { get; set; }
    public string Content { get; set; }
    public bool IsCorrect { get; set; }

    public int QuestionId { get; set; }
    public Question Question { get; set; }
}
