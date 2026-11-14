
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

namespace BeFit.Models;

public class WorkoutExercise
{
    [Key]
    public int Id { get; set; }

    [Range(1, int.MaxValue)]
    [Display(Name = "Typ ćwiczenia")]
    public int ExerciseTypeId { get; set; }

    [Range(1, int.MaxValue)]
    [Display(Name = "Sesja treningowa")]
    public int TrainingSessionId { get; set; }

    [Precision(8, 2)]
    [Range(typeof(decimal), "0", "500")]
    [Display(Name = "Obciążenie (kg)")]
    public decimal WeightKg { get; set; }

    [Range(1, 100)]
    [Display(Name = "Liczba serii")]
    public int Sets { get; set; }

    [Range(1, 500)]
    [Display(Name = "Powtórzenia w serii")]
    public int RepsPerSet { get; set; }

    [Required]
    [StringLength(450)]
    [ValidateNever]
    public string UserId { get; set; } = string.Empty;

    [ValidateNever]
    public IdentityUser User { get; set; } = null!;

    [ValidateNever]
    public ExerciseType ExerciseType { get; set; } = null!;

    [ValidateNever]
    public TrainingSession TrainingSession { get; set; } = null!;
}
