using MediatR;
using QuizAI.Application.Common;

namespace QuizAI.Application.Questions.Commands.DeleteQuestion;

public class DeleteQuestionCommand : IRequest<NewQuizId>
{
    public Guid QuizId { get; set; }
    public int QuestionId { get; set; }
}
