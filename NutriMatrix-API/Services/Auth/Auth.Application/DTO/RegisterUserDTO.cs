using System.ComponentModel.DataAnnotations;

namespace Auth.Application.DTO
{
    public class RegisterUserDTO
    {
        [Required]
        [MinLength(1)]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
        [Required]
        public string[] Roles { get; set; }
    }
}
