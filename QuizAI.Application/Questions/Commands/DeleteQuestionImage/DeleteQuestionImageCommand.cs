using MediatR;
using Microsoft.AspNetCore.Http;
using QuizAI.Application.Common;

namespace QuizAI.Application.Questions.Commands.DeleteQuestionImage;

public class DeleteQuestionImageCommand : IRequest<NewQuizId>
{
    public Guid QuizId { get; set; }
    public int QuestionId { get; set; }
}
