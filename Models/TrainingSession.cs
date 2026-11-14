
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BeFit.Models;

public class TrainingSession : IValidatableObject
{
    [Key]
    public int Id { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    [Display(Name = "Rozpoczęcie treningu")]
    public DateTime StartedAt { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    [Display(Name = "Zakończenie treningu")]
    public DateTime EndedAt { get; set; }

    [Required]
    [StringLength(450)]
    [ValidateNever]
    public string UserId { get; set; } = string.Empty;

    [ValidateNever]
    public IdentityUser User { get; set; } = null!;

    [ValidateNever]
    public ICollection<WorkoutExercise> WorkoutExercises
    { get; set; } = new List<WorkoutExercise>();

    public IEnumerable<ValidationResult> Validate(
        ValidationContext context)
    {
        if (StartedAt < new DateTime(2020, 1, 1) ||
            StartedAt > DateTime.Now.AddYears(1))
            yield return new ValidationResult(
                "Nieprawidłowa data rozpoczęcia.",
                new[] { nameof(StartedAt) });

        if (EndedAt <= StartedAt)
            yield return new ValidationResult(
                "Trening musi zakończyć się po rozpoczęciu.",
                new[] { nameof(EndedAt) });

        if (EndedAt - StartedAt > TimeSpan.FromHours(24))
            yield return new ValidationResult(
                "Trening nie może trwać ponad 24 godziny.",
                new[] { nameof(EndedAt) });
    }
}
