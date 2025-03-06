using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizAI.Application.Common;
using QuizAI.Application.Users.Commands.ChangeUserPassword;
using QuizAI.Application.Users.Commands.LoginUser;
using QuizAI.Application.Users.Commands.RefreshUserTokens;
using QuizAI.Application.Users.Commands.RegisterUser;
using QuizAI.Application.Users.Dtos;
using QuizAI.Application.Users.Queries.GetUserInfo;

namespace QuizAI.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RegisterUser(RegisterUserCommand command)
        {
            await _mediator.Send(command);
            return CreatedAtAction(nameof(LoginUser), null);
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> LoginUser(LoginUserCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }

        [Authorize]
        [HttpPost("refresh")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshUserTokens()
        {
            var refreshToken = HttpContext.Request.Cookies["REFRESH_TOKEN"];
         
            var command = new RefreshUserTokensCommand(refreshToken);
            await _mediator.Send(command);
            
            return NoContent();
        }

        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserDto>> GetUserInfo()
        {
            var userDto = await _mediator.Send(new GetUserInfoQuery());
            return Ok(userDto);
        }

        [Authorize]
        [HttpPatch("change-password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> ChangeUserPassword(ChangeUserPasswordCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmUserEmail()
        {
            return NoContent(); // TODO
        }

        [HttpPost("resend-confirmation-email")]
        public async Task<IActionResult> ResendUserConfirmationEmail()
        {
            return NoContent(); // TODO
        }

    }
}
