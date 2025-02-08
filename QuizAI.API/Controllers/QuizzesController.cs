using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizAI.Application.Common;
using QuizAI.Application.Quizzes.Commands.CreateQuiz;
using QuizAI.Application.Quizzes.Commands.DeleteQuiz;
using QuizAI.Application.Quizzes.Commands.UpdateQuiz;
using QuizAI.Application.Quizzes.Dtos;
using QuizAI.Application.Quizzes.Queries.GetAllQuizzes;
using QuizAI.Application.Quizzes.Queries.GetQuizById;

namespace QuizAI.API.Controllers;

[Route("api/quizzes")]
[ApiController]
[Authorize]
public class QuizzesController : ControllerBase
{
    private readonly IMediator _mediator;

    public QuizzesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [RequestSizeLimit(5 * 1024 * 1024)] // 5MB
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesErrorResponseType(typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status413RequestEntityTooLarge)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateQuiz(CreateQuizCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetQuiz), new { quizId = id }, null);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesErrorResponseType(typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResponse<QuizDto>>> GetAllQuizzes([FromQuery] GetAllQuizzesQuery query)
    {
        var quizzes = await _mediator.Send(query);
        return Ok(quizzes);
    }

    [HttpGet("{quizId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesErrorResponseType(typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<QuizDto>> GetQuiz(Guid quizId)
    {
        var quiz = await _mediator.Send(new GetQuizByIdQuery(quizId));
        return Ok(quiz);
    }

    [HttpPut("{quizId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesErrorResponseType(typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<NewQuizId>> UpdateQuiz(Guid quizId, UpdateQuizCommand command)
    {
        command.SetId(quizId);

        var newQuizId = await _mediator.Send(command);
        return Ok(newQuizId);
    }

    [HttpDelete("{quizId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesErrorResponseType(typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteQuiz(Guid quizId)
    {
        var command = new DeleteQuizCommand();
        command.SetId(quizId);
        
        await _mediator.Send(command);
        return NoContent();
    }
}
