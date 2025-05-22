using System.ComponentModel.DataAnnotations;

namespace Auth.API.Models.DTO
{
    public class UpdateUserDTO
    {
        [Required]
        [MinLength(1)]
        public string Name { get; set; }

        public IEnumerable<NutrientTracking> UpdateNutrients { get; set; }
    }
}
