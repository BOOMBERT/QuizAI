using MediatR;
using QuizAI.Application.MultipleChoiceQuestions.Dtos;

namespace QuizAI.Application.Questions.Commands.AnswerCurrentQuestion;

public class AnswerCurrentQuestionCommand : IRequest
{
    private Guid QuizId { get; set; }
    public ICollection<string> UserAnswer { get; set; } = new List<string>();

    public void SetQuizId(Guid id)
    {
        QuizId = id;
    }

    public Guid GetQuizId()
    {
        return QuizId;
    }
}
