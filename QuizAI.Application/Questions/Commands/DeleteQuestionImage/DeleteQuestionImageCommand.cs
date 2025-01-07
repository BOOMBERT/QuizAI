using MediatR;
using Microsoft.AspNetCore.Http;

namespace QuizAI.Application.Questions.Commands.DeleteQuestionImage;

public class DeleteQuestionImageCommand : IRequest
{
    public Guid QuizId { get; set; }
    public int QuestionId { get; set; }
}
