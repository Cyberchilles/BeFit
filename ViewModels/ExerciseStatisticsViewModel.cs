using System.ComponentModel.DataAnnotations;

namespace BeFit.ViewModels;

public class ExerciseStatisticsViewModel
{
    public List<ExerciseStatisticsRow> Rows { get; set; } = new();
}

public class ExerciseStatisticsRow
{
    [Display(Name = "Typ ćwiczenia")]
    public string ExerciseName { get; set; } = string.Empty;

    [Display(Name = "Liczba wykonań")]
    public int TimesPerformed { get; set; }

    [Display(Name = "Łączna liczba powtórzeń")]
    public int TotalRepetitions { get; set; }

    [Display(Name = "Średnie obciążenie (kg)")]
    public decimal AverageWeightKg { get; set; }

    [Display(Name = "Maksymalne obciążenie (kg)")]
    public decimal MaxWeightKg { get; set; }
}
