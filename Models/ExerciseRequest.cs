using System.ComponentModel.DataAnnotations;

namespace BeFitApp.Models
{
    public class ExerciseRequest
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Requested Exercise Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Why should we add this?")]
        public string? Reason { get; set; }

        // Link to the user who requested it
        public string RequestedById { get; set; } = string.Empty;
        public AppUser? RequestedBy { get; set; }

        public bool IsApproved { get; set; } = false;
    }
}