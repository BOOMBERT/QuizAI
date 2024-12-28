using MediatR;

namespace QuizAI.Application.TrueFalseQuestions.Commands.CreateTrueFalseQuestion;

public class CreateTrueFalseQuestionCommand : IRequest<int>
{
    private Guid QuizId { get; set; }
    public required string Content { get; set; }
    public bool IsCorrect { get; set; }

    public void SetQuizId(Guid id)
    {
        QuizId = id;
    }

    public Guid GetQuizId()
    {
        return QuizId;
    }
}
