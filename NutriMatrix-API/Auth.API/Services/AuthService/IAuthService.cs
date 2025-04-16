using Auth.API.Models.DTO;
using Microsoft.AspNetCore.Identity;

namespace Auth.API.Services.AuthService
{
    public interface IAuthService
    {
        public Task<IdentityResult> Register(RegisterUserDTO dto);
        public Task<LoginResponseDTO> Login(LoginUserDTO dto);
        public Task<bool> ConfirmEmail(RequestConfirmEmailDTO dto);
        public Task<bool> RequestPasswordReset(string email);
        public Task<bool> ResetPassword(ResetPasswordDTO dto);
    }
}
