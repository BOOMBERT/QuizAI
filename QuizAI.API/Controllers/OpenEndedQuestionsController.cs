using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizAI.Application.Common;
using QuizAI.Application.OpenEndedQuestions.Commands.CreateOpenEndedQuestion;
using QuizAI.Application.OpenEndedQuestions.Commands.UpdateOpenEndedQuestion;
using QuizAI.Application.OpenEndedQuestions.Dtos;
using QuizAI.Application.OpenEndedQuestions.Queries.GenerateOpenEndedQuestion;

namespace QuizAI.API.Controllers
{
    [Route("api/quizzes")]
    [ApiController]
    [Authorize]
    public class OpenEndedQuestionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OpenEndedQuestionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("{quizId}/questions/open-ended")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LatestQuizId>> CreateOpenEndedQuestion(Guid quizId, CreateOpenEndedQuestionCommand command)
        {
            command.SetQuizId(quizId);

            var newQuizId = await _mediator.Send(command);
            return Ok(newQuizId);
        }

        [HttpPut("{quizId}/questions/open-ended/{questionId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LatestQuizId>> UpdateOpenEndedQuestion(Guid quizId, int questionId, UpdateOpenEndedQuestionCommand command)
        {
            command.SetQuizId(quizId);
            command.SetQuestionId(questionId);

            var newQuizId = await _mediator.Send(command);
            return Ok(newQuizId);
        }

        [HttpGet("{quizId}/questions/open-ended/generate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<OpenEndedAnswersWithQuestionDto>> GenerateOpenEndedQuestion(Guid quizId, string? suggestions)
        {
            var generatedQuestion = await _mediator.Send(new GenerateOpenEndedQuestionQuery(quizId, suggestions));
            return Ok(generatedQuestion);
        }
    }
}
