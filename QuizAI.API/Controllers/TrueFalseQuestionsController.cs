using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizAI.Application.Common;
using QuizAI.Application.TrueFalseQuestions.Commands.CreateTrueFalseQuestion;
using QuizAI.Application.TrueFalseQuestions.Commands.UpdateTrueFalseQuestion;
using QuizAI.Application.TrueFalseQuestions.Dtos;
using QuizAI.Application.TrueFalseQuestions.Queries.GenerateTrueFalseQuestion;

namespace QuizAI.API.Controllers
{
    [Route("api/quizzes")]
    [ApiController]
    [Authorize]
    public class TrueFalseQuestionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TrueFalseQuestionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("{quizId}/questions/true-false")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LatestQuizId>> CreateTrueFalseQuestion(Guid quizId, CreateTrueFalseQuestionCommand command)
        {
            command.SetQuizId(quizId);

            var newQuizId = await _mediator.Send(command);
            return Ok(newQuizId);
        }

        [HttpPut("{quizId}/questions/true-false/{questionId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LatestQuizId>> UpdateTrueFalseQuestion(Guid quizId, int questionId, UpdateTrueFalseQuestionCommand command)
        {
            command.SetQuizId(quizId);
            command.SetQuestionId(questionId);

            var newQuizId = await _mediator.Send(command);
            return Ok(newQuizId);
        }

        [HttpGet("{quizId}/questions/true-false/generate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TrueFalseAnswerWithQuestionDto>> GenerateTrueFalseQuestion(Guid quizId, string? suggestions)
        {
            var generatedQuestion = await _mediator.Send(new GenerateTrueFalseQuestionQuery(quizId, suggestions));
            return Ok(generatedQuestion);
        }
    }
}
