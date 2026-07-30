using System.Linq;
using FitnessApp.Api.Data;
using FitnessApp.Api.DTOs;
using FitnessApp.Api.Models;
using FitnessApp.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Api.Services.Implementations;

public class WorkoutPlanService : IWorkoutPlanService
{
    private readonly IUnitOfWork _unitOfWork;

    public WorkoutPlanService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<WorkoutPlanReadDto>> GetAllAsync(string userId, bool isAdmin)
    {
        var query = _unitOfWork.WorkoutPlans.Query()
            .AsNoTracking()
            .Include(x => x.WorkoutPlanExercises)
                .ThenInclude(x => x.Exercise)
            .AsQueryable();

        if (!isAdmin)
        {
            query = query.Where(x => x.UserId == userId);
        }

        return await query
            .Select(x => new WorkoutPlanReadDto
            {
                Id = x.Id,
                UserId = x.UserId,
                Name = x.Name,
                Description = x.Description,
                CreatedAt = x.CreatedAt,
                Exercises = x.WorkoutPlanExercises
                    .OrderBy(e => e.DisplayOrder)
                    .Select(e => new WorkoutPlanExerciseReadDto
                    {
                        ExerciseId = e.ExerciseId,
                        ExerciseName = e.Exercise.Name,
                        DisplayOrder = e.DisplayOrder,
                        Sets = e.Sets,
                        Reps = e.Reps,
                        RestSeconds = e.RestSeconds
                    })
                    .ToList()
            })
            .ToListAsync();
    }

    public async Task<WorkoutPlanReadDto?> GetByIdAsync(int id, string userId, bool isAdmin)
    {
        var query = _unitOfWork.WorkoutPlans.Query()
            .AsNoTracking()
            .Include(x => x.WorkoutPlanExercises)
                .ThenInclude(x => x.Exercise)
            .Where(x => x.Id == id)
            .AsQueryable();

        if (!isAdmin)
        {
            query = query.Where(x => x.UserId == userId);
        }

        return await query
            .Select(x => new WorkoutPlanReadDto
            {
                Id = x.Id,
                UserId = x.UserId,
                Name = x.Name,
                Description = x.Description,
                CreatedAt = x.CreatedAt,
                Exercises = x.WorkoutPlanExercises
                    .OrderBy(e => e.DisplayOrder)
                    .Select(e => new WorkoutPlanExerciseReadDto
                    {
                        ExerciseId = e.ExerciseId,
                        ExerciseName = e.Exercise.Name,
                        DisplayOrder = e.DisplayOrder,
                        Sets = e.Sets,
                        Reps = e.Reps,
                        RestSeconds = e.RestSeconds
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<WorkoutPlanReadDto> CreateAsync(string userId, WorkoutPlanCreateUpdateDto dto)
    {
        if (dto.Exercises.Count == 0)
        {
            throw new ArgumentException("Plan treninga mora sadržavati barem jednu vježbu.");
        }

        var exerciseIds = dto.Exercises.Select(x => x.ExerciseId).Distinct().ToList();
        var existingExercisesCount = await _unitOfWork.Exercises.Query()
            .CountAsync(x => exerciseIds.Contains(x.Id));

        if (existingExercisesCount != exerciseIds.Count)
        {
            throw new ArgumentException("Jedna ili više vježbi ne postoji.");
        }

        var workoutPlan = new WorkoutPlan
        {
            UserId = userId,
            Name = dto.Name,
            Description = dto.Description,
            WorkoutPlanExercises = dto.Exercises
                .OrderBy(x => x.DisplayOrder)
                .Select(x => new WorkoutPlanExercise
                {
                    ExerciseId = x.ExerciseId,
                    DisplayOrder = x.DisplayOrder,
                    Sets = x.Sets,
                    Reps = x.Reps,
                    RestSeconds = x.RestSeconds
                })
                .ToList()
        };

        if (dto.Exercises.Select(x => x.ExerciseId).Distinct().Count() != dto.Exercises.Count)
        {
            throw new ArgumentException("Ista vježba ne smije biti dodana više puta u plan.");
        }

        if (dto.Exercises.Select(x => x.DisplayOrder).Distinct().Count() != dto.Exercises.Count)
        {
            throw new ArgumentException("Redoslijed vježbi mora biti jedinstven.");
        }

        await _unitOfWork.WorkoutPlans.AddAsync(workoutPlan);
        await _unitOfWork.SaveChangesAsync();

        var created = await GetByIdAsync(workoutPlan.Id, userId, true);
        if (created == null)
        {
            throw new InvalidOperationException("Plan treninga nije mogao biti učitan nakon spremanja.");
        }

        return created;
    }

    public async Task<bool> UpdateAsync(int id, string userId, bool isAdmin, WorkoutPlanCreateUpdateDto dto)
    {
        var workoutPlan = await _unitOfWork.WorkoutPlans.Query()
            .Include(x => x.WorkoutPlanExercises)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (workoutPlan == null)
        {
            return false;
        }

        if (!isAdmin && workoutPlan.UserId != userId)
        {
            return false;
        }

        if (dto.Exercises.Count == 0)
        {
            throw new ArgumentException("Plan treninga mora sadržavati barem jednu vježbu.");
        }

        var exerciseIds = dto.Exercises.Select(x => x.ExerciseId).Distinct().ToList();
        var existingExercisesCount = await _unitOfWork.Exercises.Query()
            .CountAsync(x => exerciseIds.Contains(x.Id));

        if (existingExercisesCount != exerciseIds.Count)
        {
            throw new ArgumentException("Jedna ili više vježbi ne postoji.");
        }

        workoutPlan.Name = dto.Name;
        workoutPlan.Description = dto.Description;

        foreach (var item in workoutPlan.WorkoutPlanExercises.ToList())
        {
            _unitOfWork.WorkoutPlanExercises.Remove(item);
        }

        foreach (var exercise in dto.Exercises.OrderBy(x => x.DisplayOrder))
        {
            await _unitOfWork.WorkoutPlanExercises.AddAsync(new WorkoutPlanExercise
            {
                WorkoutPlanId = workoutPlan.Id,
                ExerciseId = exercise.ExerciseId,
                DisplayOrder = exercise.DisplayOrder,
                Sets = exercise.Sets,
                Reps = exercise.Reps,
                RestSeconds = exercise.RestSeconds
            });
        }

        if (dto.Exercises.Select(x => x.ExerciseId).Distinct().Count() != dto.Exercises.Count)
        {
            throw new ArgumentException("Ista vježba ne smije biti dodana više puta u plan.");
        }

        if (dto.Exercises.Select(x => x.DisplayOrder).Distinct().Count() != dto.Exercises.Count)
        {
            throw new ArgumentException("Redoslijed vježbi mora biti jedinstven.");
        }

        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id, string userId, bool isAdmin)
    {
        var workoutPlan = await _unitOfWork.WorkoutPlans.Query()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (workoutPlan == null)
        {
            return false;
        }

        if (!isAdmin && workoutPlan.UserId != userId)
        {
            return false;
        }

        _unitOfWork.WorkoutPlans.Remove(workoutPlan);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}