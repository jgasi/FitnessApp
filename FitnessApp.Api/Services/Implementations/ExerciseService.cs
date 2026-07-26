using FitnessApp.Api.Data;
using FitnessApp.Api.DTOs;
using FitnessApp.Api.Models;
using FitnessApp.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Api.Services.Implementations;

public class ExerciseService : IExerciseService
{
    private readonly IUnitOfWork _unitOfWork;

    public ExerciseService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ExerciseReadDto>> GetAllAsync(string? search = null, int? exerciseCategoryId = null, int? muscleGroupId = null)
    {
        var query = _unitOfWork.Exercises.Query()
            .AsNoTracking()
            .Include(e => e.ExerciseCategory)
            .Include(e => e.MuscleGroup)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(e => e.Name.Contains(search));
        }

        if (exerciseCategoryId.HasValue)
        {
            query = query.Where(e => e.ExerciseCategoryId == exerciseCategoryId.Value);
        }

        if (muscleGroupId.HasValue)
        {
            query = query.Where(e => e.MuscleGroupId == muscleGroupId.Value);
        }

        return await query
            .Select(e => new ExerciseReadDto
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                YoutubeUrl = e.YoutubeUrl,
                ExerciseCategoryId = e.ExerciseCategoryId,
                ExerciseCategoryName = e.ExerciseCategory.Name,
                MuscleGroupId = e.MuscleGroupId,
                MuscleGroupName = e.MuscleGroup.Name
            })
            .ToListAsync();
    }

    public async Task<ExerciseReadDto?> GetByIdAsync(int id)
    {
        return await _unitOfWork.Exercises.Query()
            .AsNoTracking()
            .Include(e => e.ExerciseCategory)
            .Include(e => e.MuscleGroup)
            .Where(e => e.Id == id)
            .Select(e => new ExerciseReadDto
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                YoutubeUrl = e.YoutubeUrl,
                ExerciseCategoryId = e.ExerciseCategoryId,
                ExerciseCategoryName = e.ExerciseCategory.Name,
                MuscleGroupId = e.MuscleGroupId,
                MuscleGroupName = e.MuscleGroup.Name
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ExerciseReadDto> CreateAsync(ExerciseCreateUpdateDto dto)
    {
        var categoryExists = await _unitOfWork.ExerciseCategories.Query().AnyAsync(x => x.Id == dto.ExerciseCategoryId);
        if (!categoryExists)
        {
            throw new ArgumentException("Kategorija vježbe ne postoji.");
        }

        var muscleGroupExists = await _unitOfWork.MuscleGroups.Query().AnyAsync(x => x.Id == dto.MuscleGroupId);
        if (!muscleGroupExists)
        {
            throw new ArgumentException("Mišićna skupina ne postoji.");
        }

        var exercise = new Exercise
        {
            Name = dto.Name,
            Description = dto.Description,
            YoutubeUrl = dto.YoutubeUrl,
            ExerciseCategoryId = dto.ExerciseCategoryId,
            MuscleGroupId = dto.MuscleGroupId
        };

        await _unitOfWork.Exercises.AddAsync(exercise);
        await _unitOfWork.SaveChangesAsync();

        var created = await GetByIdAsync(exercise.Id);
        if (created == null)
        {
            throw new InvalidOperationException("Vježba nije mogla biti učitana nakon spremanja.");
        }

        return created;
    }

    public async Task<bool> UpdateAsync(int id, ExerciseCreateUpdateDto dto)
    {
        var exercise = await _unitOfWork.Exercises.GetByIdAsync(id);
        if (exercise == null)
        {
            return false;
        }

        var categoryExists = await _unitOfWork.ExerciseCategories.Query().AnyAsync(x => x.Id == dto.ExerciseCategoryId);
        if (!categoryExists)
        {
            throw new ArgumentException("Kategorija vježbe ne postoji.");
        }

        var muscleGroupExists = await _unitOfWork.MuscleGroups.Query().AnyAsync(x => x.Id == dto.MuscleGroupId);
        if (!muscleGroupExists)
        {
            throw new ArgumentException("Mišićna skupina ne postoji.");
        }

        exercise.Name = dto.Name;
        exercise.Description = dto.Description;
        exercise.YoutubeUrl = dto.YoutubeUrl;
        exercise.ExerciseCategoryId = dto.ExerciseCategoryId;
        exercise.MuscleGroupId = dto.MuscleGroupId;

        _unitOfWork.Exercises.Update(exercise);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var exercise = await _unitOfWork.Exercises.GetByIdAsync(id);
        if (exercise == null)
        {
            return false;
        }

        _unitOfWork.Exercises.Remove(exercise);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}