
using System.ComponentModel.DataAnnotations;

namespace BeFit.Models;

public class ExerciseType
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Podaj nazwę ćwiczenia.")]
    [StringLength(80, MinimumLength = 2)]
    [Display(Name = "Nazwa ćwiczenia")]
    public string Name { get; set; } = string.Empty;

    public ICollection<WorkoutExercise> WorkoutExercises
    { get; set; } = new List<WorkoutExercise>();
}
