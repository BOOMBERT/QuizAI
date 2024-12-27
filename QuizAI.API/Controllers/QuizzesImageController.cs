using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuizAI.Application.Common;
using QuizAI.Application.Quizzes.Commands.DeleteQuizImage;
using QuizAI.Application.Quizzes.Commands.UpdateQuizImage;
using QuizAI.Application.Quizzes.Queries.GetQuizImageById;

namespace QuizAI.API.Controllers
{
    [Route("api/quizzes")]
    [ApiController]
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
        public async Task<IActionResult> GetQuizImage(Guid quizId)
        {
            var (imageData, contextType) = await _mediator.Send(new GetQuizImageByIdQuery(quizId));
            return File(imageData, contextType);
        }

        [HttpPut("{quizId}/image")]
        [RequestSizeLimit(5 * 1024 * 1024)] // 5MB
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status413RequestEntityTooLarge)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        public async Task<IActionResult> UpdateQuizImage(Guid quizId, UpdateQuizImageCommand command)
        {
            command.SetId(quizId);

            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{quizId}/image")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteQuizImage(Guid quizId, DeleteQuizImageCommand command)
        {
            command.SetId(quizId);

            await _mediator.Send(command);
            return NoContent();
        }
    }
}
