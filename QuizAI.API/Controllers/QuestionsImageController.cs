using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizAI.Application.Common;
using QuizAI.Application.QuestionImages.Commands.DeleteQuestionImage;
using QuizAI.Application.QuestionImages.Commands.UpdateQuestionImage;
using QuizAI.Application.QuestionImages.Queries.GetQuestionImageById;

namespace QuizAI.API.Controllers
{
    [Route("api/quizzes")]
    [ApiController]
    [Authorize]
    public class QuestionsImageController : ControllerBase
    {
        private readonly IMediator _mediator;

        public QuestionsImageController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{quizId}/questions/{questionId}/image")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetQuestionImage(Guid quizId, int questionId)
        {
            var (imageData, contextType) = await _mediator.Send(new GetQuestionImageByIdQuery(quizId, questionId));
            return File(imageData, contextType);
        }

        [HttpPatch("{quizId}/questions/{questionId}/image")]
        [RequestSizeLimit(5 * 1024 * 1024)] // 5MB
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status413RequestEntityTooLarge)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LatestQuizId>> UpdateQuestionImage(Guid quizId, int questionId, UpdateQuestionImageCommand command)
        {
            command.SetQuizId(quizId);
            command.SetQuestionId(questionId);

            var newQuizId = await _mediator.Send(command);
            return Ok(newQuizId);
        }

        [HttpDelete("{QuizId}/questions/{QuestionId}/image")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LatestQuizId>> DeleteQuestionImage([FromRoute] DeleteQuestionImageCommand command)
        {
            var newQuizId = await _mediator.Send(command);
            return Ok(newQuizId);
        }
    }
}
