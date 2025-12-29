using Auth.Application.DTO;
using Auth.Application.Features.Auth.Commands;
using Auth.Application.Features.Auth.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDTO dto)
        {
            var command = new RegisterCommand { RegisterUserDTO = dto };
            var result = await _mediator.Send(command);

            if (!result.Succeeded) return BadRequest(new { Message = "Couldn't register user", Errors = result.Errors });

            return Ok(new { Message = "User successfully created" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserDTO dto)
        {
            var query = new LoginQuery { LoginUserDTO = dto };
            var result = await _mediator.Send(query);

            if (result == null) return BadRequest("Couldn't login user");

            return Ok(result);
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(RequestConfirmEmailDTO dto)
        {
            var command = new ConfirmEmailCommand { RequestConfirmEmailDTO = dto };
            var result = await _mediator.Send(command);

            if (!result) return BadRequest("Couldn't confirm email");

            return Ok("Email confirmed successfully");
        }

        [HttpPost("request-reset-password")]
        public async Task<IActionResult> RequestResetPassword(string email)
        {
            var command = new RequestPasswordResetCommand { Email = email };
            var result = await _mediator.Send(command);

            if (!result) return BadRequest("Couldn't send password reset request");

            return Ok("Password reset sent successfully");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO dto)
        {
            var command = new ResetPasswordCommand { ResetPasswordDTO = dto };
            var result = await _mediator.Send(command);

            if (!result) return BadRequest("Couldn't reset password");

            return Ok("Password reset successfully");
        }

        [HttpPost("login/google")]
        public async Task<IActionResult> LoginViaGoogle(string idToken)
        {
            var command = new GoogleLoginCommand { IdToken = idToken };
            var result = await _mediator.Send(command);

            if (result == null) return BadRequest("Oops, something went wrong");

            return Ok(result);
        }
    }
}