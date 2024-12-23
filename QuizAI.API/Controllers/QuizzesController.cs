using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuizAI.Application.Common;
using QuizAI.Application.Quizzes.Commands.CreateQuiz;
using QuizAI.Application.Quizzes.Commands.UpdateQuiz;
using QuizAI.Application.Quizzes.Dtos;
using QuizAI.Application.Quizzes.Queries.GetQuizById;

namespace QuizAI.API.Controllers;

[Route("api/quizzes")]
[ApiController]
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
    public async Task<IActionResult> CreateQuiz(CreateQuizCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetQuiz), new { quizId = id }, null);
    }

    [HttpPut("{quizId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesErrorResponseType(typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateQuiz(Guid quizId, UpdateQuizCommand command)
    {
        command.SetId(quizId);

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpGet("{quizId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesErrorResponseType(typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<QuizDto>> GetQuiz(Guid quizId)
    {
        var quiz = await _mediator.Send(new GetQuizByIdQuery(quizId));
        return Ok(quiz);
    }
}
