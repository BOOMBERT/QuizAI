using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizAI.Application.Common;
using QuizAI.Application.QuizImages.Commands.DeleteQuizImage;
using QuizAI.Application.QuizImages.Commands.UpdateQuizImage;
using QuizAI.Application.QuizImages.Queries.GetQuizImageById;

namespace QuizAI.API.Controllers
{
    [Route("api/quizzes")]
    [ApiController]
    [Authorize]
    public class QuizzesImageController : ControllerBase
    {
        private readonly IMediator _mediator;

        public QuizzesImageController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{quizId}/image")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetQuizImage(Guid quizId)
        {
            var (imageData, contextType) = await _mediator.Send(new GetQuizImageByIdQuery(quizId));
            return File(imageData, contextType);
        }

        [HttpPatch("{quizId}/image")]
        [RequestSizeLimit(5 * 1024 * 1024)] // 5MB
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status413RequestEntityTooLarge)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LatestQuizId>> UpdateQuizImage(Guid quizId, UpdateQuizImageCommand command)
        {
            command.SetId(quizId);

            var newQuizId = await _mediator.Send(command);
            return Ok(newQuizId);
        }

        [HttpDelete("{quizId}/image")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LatestQuizId>> DeleteQuizImage(Guid quizId)
        {
            var command = new DeleteQuizImageCommand();
            command.SetId(quizId);

            var newQuizId = await _mediator.Send(command);
            return Ok(newQuizId);
        }
    }
}
