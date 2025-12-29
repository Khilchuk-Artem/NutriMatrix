using Auth.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Auth.Application.DTO
{
    public class UpdateUserDTO
    {
        [Required]
        [MinLength(1)]
        public string Name { get; set; }

        public IEnumerable<NutrientTracking> UpdateNutrients { get; set; }
    }
}
