using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizAI.Application.Common;
using QuizAI.Application.Questions.Commands.DeleteQuestion;
using QuizAI.Domain.Entities;

namespace QuizAI.API.Controllers
{
    [Route("api/quizzes/{quizId}/questions")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public QuestionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpDelete("{questionId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteQuestion(Guid quizId, int questionId)
        {
            var command = new DeleteQuestionCommand();
            command.SetQuizId(quizId);
            command.SetQuestionId(questionId);

            await _mediator.Send(command);
            return NoContent();
        }
    }
}
