using Auth.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Auth.Application.DTO
{
    public class LoginResponseDTO
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Token { get; set; }
        [Required]
        public string[] Roles { get; set; }
        public NutrientTracking[] NutrientTrackings { get; set; }
    }
}
