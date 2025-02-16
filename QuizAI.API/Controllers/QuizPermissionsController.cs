using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizAI.Application.Common;
using QuizAI.Application.QuizPermissions.Commands.ManageUsersQuizPermissions;
using QuizAI.Application.QuizPermissions.Dtos;
using QuizAI.Application.QuizPermissions.Queries.GetAllQuizUsersPermissions;

namespace QuizAI.API.Controllers
{
    [Route("api/quizzes/{quizId}/permissions")]
    [ApiController]
    [Authorize]
    public class QuizPermissionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public QuizPermissionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPut("{userEmail}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ManageUsersQuizPermissions(Guid quizId, string userEmail, ManageUsersQuizPermissionsCommand command)
        {
            command.SetQuizId(quizId);
            command.SetUserEmail(userEmail);

            await _mediator.Send(command);
            return NoContent();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<QuizUsersPermissionsDto>>> GetAllQuizUsersPermissions(Guid quizId)
        {
            var quizUsersPermissions = await _mediator.Send(new GetAllQuizUsersPermissionsQuery(quizId));
            return Ok(quizUsersPermissions);
        }
    }
}
