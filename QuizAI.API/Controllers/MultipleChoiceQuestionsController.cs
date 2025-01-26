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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<NewQuizId>> CreateMultipleChoiceQuestion(Guid quizId, CreateMultipleChoiceQuestionCommand command)
        {
            command.SetQuizId(quizId);

            var newQuizId = await _mediator.Send(command);
            return Ok(newQuizId);
        }

        [HttpPut("{quizId}/questions/multiple-choice/{questionId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<NewQuizId>> UpdateMultipleChoiceQuestion(Guid quizId, int questionId, UpdateMultipleChoiceQuestionCommand command)
        {
            command.SetQuizId(quizId);
            command.SetQuestionId(questionId);

            var newQuizId = await _mediator.Send(command);
            return Ok(newQuizId);
        }
    }
}
