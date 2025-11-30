using System.ComponentModel.DataAnnotations;

namespace BeFitApp.Models
{
    public class Exercise
    {
        public int Id { get; set; }

        public int TrainingSessionId { get; set; }
        public TrainingSession? TrainingSession { get; set; }

        [Required]
        [Display(Name = "Exercise Type")]
        public int ExerciseTypeId { get; set; }
        public ExerciseType? ExerciseType { get; set; }
        
        [Range(0, 1000)]
        [Display(Name = "Weight (kg)")]
        public double Weight { get; set; }

        [Range(1, 100)]
        [Display(Name = "Sets")]
        public int Sets { get; set; }

        [Range(1, 500)]
        [Display(Name = "Reps per Set")]
        public int Repetitions { get; set; }
    }
}