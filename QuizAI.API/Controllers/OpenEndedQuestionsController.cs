using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuizAI.Application.Common;
using QuizAI.Application.MultipleChoiceQuestions.Commands.UpdateMultipleChoiceQuestion;
using QuizAI.Application.OpenEndedQuestions.Commands.CreateOpenEndedQuestion;
using QuizAI.Application.OpenEndedQuestions.Commands.UpdateOpenEndedQuestion;

namespace QuizAI.API.Controllers
{
    [Route("api/quizzes")]
    [ApiController]
    public class OpenEndedQuestionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OpenEndedQuestionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("{quizId}/questions/open-ended")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateOpenEndedQuestion(Guid quizId, CreateOpenEndedQuestionCommand command)
        {
            command.SetQuizId(quizId);

            var order = await _mediator.Send(command);
            return CreatedAtAction("GetQuestionByOrder", "Questions", new { QuizId = quizId, orderNumber = order }, null);
        }

        [HttpPut("{quizId}/questions/open-ended/{questionId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateOpenEndedQuestion(Guid quizId, int questionId, UpdateOpenEndedQuestionCommand command)
        {
            command.SetQuizId(quizId);
            command.SetQuestionId(questionId);

            await _mediator.Send(command);
            return NoContent();
        }
    }
}
