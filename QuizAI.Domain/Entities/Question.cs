using QuizAI.Domain.Enums;

namespace QuizAI.Domain.Entities;

public class Question
{
    public int Id { get; set; }
    public string Content { get; set; }
    public QuestionType Type { get; set; }
    public byte Order { get; set; }

    public int? ImageId { get; set; }
    public Image? Image { get; set; }

    public Guid QuizId { get; set; }
    public Quiz Quiz { get; set; }

    public ICollection<Answer> Answers { get; set; } = new List<Answer>();
}
