using MediatR;
using QuizAI.Application.Common;

namespace QuizAI.Application.OpenEndedQuestions.Commands.CreateOpenEndedQuestion;

public class CreateOpenEndedQuestionCommand : IRequest<LatestQuizId>
{
    private Guid QuizId { get; set; }
    public string Content { get; set; } = string.Empty;
    public List<string> Answers { get; set; } = new List<string>();
    public bool VerificationByAI { get; set; }

    public void SetQuizId(Guid id)
    {
        QuizId = id;
    }

    public Guid GetQuizId()
    {
        return QuizId;
    }
}
