using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizAI.Application.Common;
using QuizAI.Application.QuizAttempts.Dtos;
using QuizAI.Application.QuizAttempts.Queries.GetLatestAttempt;
using QuizAI.Application.Quizzes.Commands.ChangeQuizPrivacy;
using QuizAI.Application.Quizzes.Commands.CreateQuiz;
using QuizAI.Application.Quizzes.Commands.DeleteQuiz;
using QuizAI.Application.Quizzes.Commands.UpdateQuiz;
using QuizAI.Application.Quizzes.Dtos;
using QuizAI.Application.Quizzes.Queries.GetAllQuizzes;
using QuizAI.Application.Quizzes.Queries.GetQuizById;
using QuizAI.Application.Quizzes.Queries.GetQuizStats;

namespace QuizAI.API.Controllers
{
    [Route("api/quizzes")]
    [ApiController]
    [Authorize]
    public class QuizzesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public QuizzesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [RequestSizeLimit(5 * 1024 * 1024)] // 5MB
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status413RequestEntityTooLarge)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateQuiz(CreateQuizCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetQuizById), new { quizId = id }, null);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<QuizDto>>> GetAllQuizzes([FromQuery] GetAllQuizzesQuery query)
        {
            var quizzes = await _mediator.Send(query);
            return Ok(quizzes);
        }

        [HttpGet("{quizId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<QuizDto>> GetQuizById(Guid quizId)
        {
            var quiz = await _mediator.Send(new GetQuizByIdQuery(quizId));
            return Ok(quiz);
        }

        [HttpPut("{quizId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LatestQuizId>> UpdateQuiz(Guid quizId, UpdateQuizCommand command)
        {
            command.SetId(quizId);

            var latestQuizId = await _mediator.Send(command);
            return Ok(latestQuizId);
        }

        [HttpPatch("{quizId}/privacy")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LatestQuizId>> ChangeQuizPrivacy(Guid quizId, ChangeQuizPrivacyCommand command)
        {
            command.SetId(quizId);

            var latestQuizId = await _mediator.Send(command);
            return Ok(latestQuizId);
        }

        [HttpDelete("{quizId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteQuiz(Guid quizId)
        {
            var command = new DeleteQuizCommand();
            command.SetId(quizId);
        
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpGet("{quizId}/attempts/latest")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<QuizAttemptViewWithUserAnsweredQuestionsDto>> GetLatestAttempt(Guid quizId)
        {
            var latestAttempt = await _mediator.Send(new GetLatestAttemptQuery(quizId));
            return Ok(latestAttempt);
        }

        [HttpGet("{quizId}/stats")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<QuizStatsDto>> GetQuizStats(Guid quizId, bool includeDeprecatedVersions)
        {
            var quizStats = await _mediator.Send(new GetQuizStatsQuery(quizId, includeDeprecatedVersions));
            return Ok(quizStats);
        }
    }
}
