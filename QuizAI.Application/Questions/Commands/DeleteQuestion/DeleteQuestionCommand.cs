using MediatR;

namespace QuizAI.Application.Questions.Commands.DeleteQuestion;

public class DeleteQuestionCommand : IRequest
{
    public Guid QuizId { get; set; }
    public int QuestionId { get; set; }
}
