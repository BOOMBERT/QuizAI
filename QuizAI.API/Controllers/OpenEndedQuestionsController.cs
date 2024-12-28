using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuizAI.Application.Common;
using QuizAI.Application.OpenEndedQuestions.Commands.CreateOpenEndedQuestion;

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

        [HttpPost("{quizId}/open-ended-questions")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateOpenEndedQuestion(Guid quizId, CreateOpenEndedQuestionCommand command)
        {
            command.SetQuizId(quizId);

            var id = await _mediator.Send(command);
            return Created();
        }
    }
}
