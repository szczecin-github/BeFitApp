using System.ComponentModel.DataAnnotations;

namespace BeFitApp.Models
{
    public class ExerciseType
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Exercise Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string? Description { get; set; }
    }
}