namespace QuizAI.Domain.Entities;

public class OpenEndedAnswer
{
    public int Id { get; set; }
    public IList<string> ValidContent { get; set; } = new List<string>();
    public bool VerificationByAI { get; set; }
    
    public int QuestionId { get; set; }
    public Question Question { get; set; }
}
