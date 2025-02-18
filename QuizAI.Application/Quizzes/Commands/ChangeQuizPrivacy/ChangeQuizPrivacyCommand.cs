using MediatR;
using QuizAI.Application.Common;

namespace QuizAI.Application.Quizzes.Commands.ChangeQuizPrivacy;

public class ChangeQuizPrivacyCommand : IRequest<LatestQuizId>
{
    private Guid Id { get; set; }
    public bool IsPrivate { get; set; }

    public void SetId(Guid id)
    {
        Id = id;
    }

    public Guid GetId()
    {
        return Id;
    }
}
