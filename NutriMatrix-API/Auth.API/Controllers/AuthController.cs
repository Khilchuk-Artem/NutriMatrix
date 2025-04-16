using Auth.API.Models.DTO;
using Auth.API.Services.AuthService;
using Auth.API.Services.EmailService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;
        public AuthController(IAuthService authService, IEmailService emailService)
        {
            _authService = authService;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDTO dto)
        {
            var result = await _authService.Register(dto);

            if (result == null) return BadRequest(new { Message = "Couldn't register user" });

            return Ok(new { Message = "User successfully created" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserDTO dto)
        {
            var result = await _authService.Login(dto);

            if (result == null) return BadRequest("Couldn't login user");

            return Ok(result);
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(RequestConfirmEmailDTO dto)
        {
            var result = await _authService.ConfirmEmail(dto);

            if (result) return Ok("Email confirmed successfully");

            return BadRequest("Couldn't confirm email");
        }

        [HttpPost("request-reset-password")]
        public async Task<IActionResult> RequestResetPassword(string email)
        {
            var result = await _authService.RequestPasswordReset(email);

            if (result) return Ok("Password reset sent successfully");

            return BadRequest("Couldn't send password reset request");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO dto)
        {
            var result = await _authService.ResetPassword(dto);

            if (result) return Ok("Password reset successfully");

            return BadRequest("Couldn't reset password");
        }
    }
}
