using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuizAI.Application.Common;
using QuizAI.Application.TrueFalseQuestions.Commands.CreateTrueFalseQuestion;

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

        [HttpPost("{quizId}/true-false-questions")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateTrueFalseQuestion(Guid quizId, CreateTrueFalseQuestionCommand command)
        {
            command.SetQuizId(quizId);

            var id = await _mediator.Send(command);
            return Created();
        }
    }
}
