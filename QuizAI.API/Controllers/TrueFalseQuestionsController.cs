using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuizAI.Application.Common;
using QuizAI.Application.MultipleChoiceQuestions.Commands.UpdateMultipleChoiceQuestion;
using QuizAI.Application.TrueFalseQuestions.Commands.CreateTrueFalseQuestion;
using QuizAI.Application.TrueFalseQuestions.Commands.UpdateTrueFalseQuestion;

namespace QuizAI.API.Controllers
{
    [Route("api/quizzes")]
    [ApiController]
    public class TrueFalseQuestionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TrueFalseQuestionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("{quizId}/questions/true-false")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateTrueFalseQuestion(Guid quizId, CreateTrueFalseQuestionCommand command)
        {
            command.SetQuizId(quizId);

            var order = await _mediator.Send(command);
            return CreatedAtAction("GetQuestionByOrder", "Questions", new { QuizId = quizId, orderNumber = order }, null);
        }

        [HttpPut("{quizId}/questions/true-false/{questionId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateTrueFalseQuestion(Guid quizId, int questionId, UpdateTrueFalseQuestionCommand command)
        {
            command.SetQuizId(quizId);
            command.SetQuestionId(questionId);

            await _mediator.Send(command);
            return NoContent();
        }
    }
}
