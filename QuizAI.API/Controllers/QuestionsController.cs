using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizAI.Application.Common;
using QuizAI.Application.Questions.Commands.DeleteQuestion;
using QuizAI.Application.Questions.Commands.UpdateQuestionOrder;
using QuizAI.Domain.Entities;

namespace QuizAI.API.Controllers
{
    [Route("api/quizzes/{QuizId}/questions")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public QuestionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpDelete("{QuestionId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteQuestion([FromRoute] DeleteQuestionCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPatch("order")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateQuestionOrder(Guid QuizId, UpdateQuestionOrderCommand command)
        {
            command.SetQuizId(QuizId);

            await _mediator.Send(command);
            return NoContent();
        }
    }
}
