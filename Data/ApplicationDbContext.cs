using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BeFit.Models;

namespace BeFit.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<ExerciseType> ExerciseTypes
    => Set<ExerciseType>();

        public DbSet<TrainingSession> TrainingSessions
            => Set<TrainingSession>();

        public DbSet<WorkoutExercise> WorkoutExercises
            => Set<WorkoutExercise>();
    }
}
