using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuizAI.Application.Quizzes.Commands.CreateQuiz;

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
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateQuiz(CreateQuizCommand command)
    {
        await _mediator.Send(command);
        return Created();
    }
}
