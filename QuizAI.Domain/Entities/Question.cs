using QuizAI.Domain.Enums;

namespace QuizAI.Domain.Entities;

public class Question
{
    public int Id { get; set; }
    public string Content { get; set; }
    public QuestionType Type { get; set; }
    public byte Order { get; set; }

    public Guid? ImageId { get; set; }
    public Image? Image { get; set; }

    public Guid QuizId { get; set; }
    public Quiz Quiz { get; set; }

    public ICollection<MultipleChoiceAnswer> MultipleChoiceAnswers { get; set; } = new List<MultipleChoiceAnswer>();
    public ICollection<OpenEndedAnswer> OpenEndedAnswers { get; set; } = new List<OpenEndedAnswer>();
    public ICollection<TrueFalseAnswer> TrueFalseAnswers { get; set; } = new List<TrueFalseAnswer>();



}
