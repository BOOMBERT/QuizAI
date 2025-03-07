namespace QuizAI.Application.Common;

public class EmailMessage
{
    public required string ToEmail { get; set; }
    public required string Subject { get; set; }
    public required string HtmlMessage { get; set; }
}
