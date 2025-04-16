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
        public AuthController(IAuthService authService)
        {
            _authService = authService;
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

            if (!result) return BadRequest("Couldn't confirm email"); 

            return Ok("Email confirmed successfully");
        }

        [HttpPost("request-reset-password")]
        public async Task<IActionResult> RequestResetPassword(string email)
        {
            var result = await _authService.RequestPasswordReset(email);

            if (!result) return BadRequest("Couldn't send password reset request");

            return Ok("Password reset sent successfully");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO dto)
        {
            var result = await _authService.ResetPassword(dto);

            if (!result) return BadRequest("Couldn't reset password"); 

            return Ok("Password reset successfully");
        }

        public async Task<IActionResult> LoginViaGoogle(string idToken)
        {
            var result = await _authService.GoogleLogin(idToken);

            if (result == null) return BadRequest("Oops, something went wrong");

            return Ok(result);
        }
    }
}
