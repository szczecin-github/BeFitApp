using System.ComponentModel.DataAnnotations;

namespace BeFitApp.Models
{
    public class FoodEntry
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Food Item")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now;

        [Range(0, 5000)]
        public int Calories { get; set; }
        
        [Range(0, 500)]
        public double Fats { get; set; }

        [Range(0, 500)]
        public double Carbs { get; set; }

        [Range(0, 500)]
        public double Sugar { get; set; }

        public string UserId { get; set; } = string.Empty;
        public AppUser? User { get; set; }
    }
}