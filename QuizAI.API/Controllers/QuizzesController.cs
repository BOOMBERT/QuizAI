using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuizAI.Application.Common;
using QuizAI.Application.Quizzes.Commands.CreateQuiz;
using QuizAI.Application.Quizzes.Queries.GetQuizImageById;

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
        await _mediator.Send(command);
        return Created();
    }

    [HttpGet("{quizId}/image")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesErrorResponseType(typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetQuizImage(Guid quizId)
    {
        var (imageData, contextType) = await _mediator.Send(new GetQuizImageByIdQuery(quizId));
        return File(imageData, contextType);
    }
}
