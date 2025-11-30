using System.ComponentModel.DataAnnotations;

namespace BeFitApp.Models
{
    public class TrainingSession
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [Display(Name = "Session Name")] 
        public string Name { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Start Time")] 
        public DateTime StartDateTime { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "End Time")] 
        public DateTime EndDateTime { get; set; } = DateTime.Now.AddHours(1);

        public string UserId { get; set; } = string.Empty;
        public AppUser? User { get; set; }

        public List<Exercise> Exercises { get; set; } = new();
    }
}