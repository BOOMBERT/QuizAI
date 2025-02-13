using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizAI.Application.Common;
using QuizAI.Application.QuizAttempts.Dtos;
using QuizAI.Application.QuizAttempts.Queries.GetAllAttempts;
using QuizAI.Application.QuizAttempts.Queries.GetAttemptById;

namespace QuizAI.API.Controllers
{
    [Route("api/quiz-attempts")]
    [ApiController]
    [Authorize]
    public class QuizAttemptsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public QuizAttemptsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{quizAttemptId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<QuizAttemptViewWithUserAnsweredQuestionsDto>> GetAttemptById(Guid quizAttemptId)
        {
            var attempt = await _mediator.Send(new GetAttemptByIdQuery(quizAttemptId));
            return Ok(attempt);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<QuizAttemptViewDto>>> GetAllAttempts([FromQuery] GetAllAttemptsQuery query)
        {
            var attempts = await _mediator.Send(query);
            return Ok(attempts);
        }
    }
}
