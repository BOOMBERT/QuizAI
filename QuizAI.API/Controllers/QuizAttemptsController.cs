using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizAI.Application.Common;
using QuizAI.Application.QuizAttempts.Dtos;
using QuizAI.Application.QuizAttempts.Queries.GetAttemptById;
using QuizAI.Application.QuizAttempts.Queries.GetLatestAttempt;

namespace QuizAI.API.Controllers
{
    [Route("api/quizzes")]
    [ApiController]
    [Authorize]
    public class QuizAttemptsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public QuizAttemptsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{quizId}/attempts/latest")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<QuizAttemptWithUserAnsweredQuestionsDto>> GetLatestAttempt(Guid quizId)
        {
            var latestAttempt = await _mediator.Send(new GetLatestAttemptQuery(quizId));
            return Ok(latestAttempt);
        }

        [HttpGet("{quizId}/attempts/{quizAttemptId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<QuizAttemptWithUserAnsweredQuestionsDto>> GetAttemptById(Guid quizId, Guid quizAttemptId)
        {
            var attempt = await _mediator.Send(new GetAttemptByIdQuery(quizId, quizAttemptId));
            return Ok(attempt);
        }
    }
}
