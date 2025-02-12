using MediatR;
using Microsoft.AspNetCore.Http;
using QuizAI.Application.Common;

namespace QuizAI.Application.QuestionImages.Commands.DeleteQuestionImage;

public class DeleteQuestionImageCommand : IRequest<LatestQuizId>
{
    public Guid QuizId { get; set; }
    public int QuestionId { get; set; }
}
