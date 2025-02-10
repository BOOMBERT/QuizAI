using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizAI.Application.Common;
using QuizAI.Application.Questions.Commands.AnswerCurrentQuestion;
using QuizAI.Application.Questions.Commands.DeleteQuestion;
using QuizAI.Application.Questions.Commands.UpdateQuestionOrder;
using QuizAI.Application.Questions.Dtos;
using QuizAI.Application.Questions.Queries.GetAllQuestions;
using QuizAI.Application.Questions.Queries.GetQuestion;
using QuizAI.Application.Questions.Queries.GetQuestionByOrder;
using QuizAI.Domain.Entities;

namespace QuizAI.API.Controllers
{
    [Route("api/quizzes/{QuizId}/questions")]
    [ApiController]
    [Authorize]
    public class QuestionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public QuestionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<QuestionWithAnswerDto>>> GetAllQuestions(Guid QuizId)
        {
            var questions = await _mediator.Send(new GetAllQuestionsQuery(QuizId));
            return Ok(questions);
        }

        [HttpGet("next")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<QuestionDto>> GetNextQuestion(Guid QuizId)
        {
            var question = await _mediator.Send(new GetNextQuestionQuery(QuizId));
            return Ok(question);
        }

        [HttpGet("order/{orderNumber}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<QuestionWithAnswerDto>> GetQuestionByOrder(Guid QuizId, int orderNumber)
        {
            var question = await _mediator.Send(new GetQuestionByOrderQuery(QuizId, orderNumber));
            return Ok(question);
        }

        [HttpPost("answer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AnswerCurrentQuestion(Guid QuizId, AnswerCurrentQuestionCommand command)
        {
            command.SetQuizId(QuizId);

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

        [HttpDelete("{QuestionId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LatestQuizId>> DeleteQuestion([FromRoute] DeleteQuestionCommand command)
        {
            var newQuizId = await _mediator.Send(command);
            return Ok(newQuizId);
        }
    }
}
