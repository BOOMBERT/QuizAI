using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuizAI.Application.Common;
using QuizAI.Application.MultipleChoiceQuestions.Commands.CreateMultipleChoiceQuestion;
using QuizAI.Application.MultipleChoiceQuestions.Commands.UpdateMultipleChoiceQuestion;

namespace QuizAI.API.Controllers
{
    [Route("api/quizzes")]
    [ApiController]
    public class MultipleChoiceQuestionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MultipleChoiceQuestionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("{quizId}/questions/multiple-choice")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateMultipleChoiceQuestion(Guid quizId, CreateMultipleChoiceQuestionCommand command)
        {
            command.SetQuizId(quizId);

            var id = await _mediator.Send(command);
            return Created();
        }

        [HttpPut("{quizId}/questions/multiple-choice/{questionId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateMultipleChoiceQuestion(Guid quizId, int questionId, UpdateMultipleChoiceQuestionCommand command)
        {
            command.SetQuizId(quizId);
            command.SetQuestionId(questionId);

            await _mediator.Send(command);
            return NoContent();
        }
    }
}
