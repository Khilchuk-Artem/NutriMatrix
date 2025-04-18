using System.ComponentModel.DataAnnotations;

namespace Auth.API.Models.DTO
{
    public class LoginUserDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}
