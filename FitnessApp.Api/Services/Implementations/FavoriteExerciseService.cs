using FitnessApp.Api.Data;
using FitnessApp.Api.DTOs;
using FitnessApp.Api.Models;
using FitnessApp.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Api.Services.Implementations;

public class FavoriteExerciseService : IFavoriteExerciseService
{
    private readonly IUnitOfWork _unitOfWork;

    public FavoriteExerciseService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<FavoriteExerciseReadDto>> GetAllAsync(string userId, bool isAdmin)
    {
        var query = _unitOfWork.FavoriteExercises.Query()
            .AsNoTracking()
            .Include(x => x.Exercise)
                .ThenInclude(x => x.ExerciseCategory)
            .Include(x => x.Exercise)
                .ThenInclude(x => x.MuscleGroup)
            .AsQueryable();

        if (!isAdmin)
        {
            query = query.Where(x => x.UserId == userId);
        }

        return await query
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new FavoriteExerciseReadDto
            {
                Id = x.Id,
                UserId = x.UserId,
                ExerciseId = x.ExerciseId,
                ExerciseName = x.Exercise.Name,
                ExerciseDescription = x.Exercise.Description,
                YoutubeUrl = x.Exercise.YoutubeUrl,
                ExerciseCategoryName = x.Exercise.ExerciseCategory.Name,
                MuscleGroupName = x.Exercise.MuscleGroup.Name,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<FavoriteExerciseReadDto?> GetByIdAsync(int id, string userId, bool isAdmin)
    {
        var query = _unitOfWork.FavoriteExercises.Query()
            .AsNoTracking()
            .Include(x => x.Exercise)
                .ThenInclude(x => x.ExerciseCategory)
            .Include(x => x.Exercise)
                .ThenInclude(x => x.MuscleGroup)
            .Where(x => x.Id == id)
            .AsQueryable();

        if (!isAdmin)
        {
            query = query.Where(x => x.UserId == userId);
        }

        return await query
            .Select(x => new FavoriteExerciseReadDto
            {
                Id = x.Id,
                UserId = x.UserId,
                ExerciseId = x.ExerciseId,
                ExerciseName = x.Exercise.Name,
                ExerciseDescription = x.Exercise.Description,
                YoutubeUrl = x.Exercise.YoutubeUrl,
                ExerciseCategoryName = x.Exercise.ExerciseCategory.Name,
                MuscleGroupName = x.Exercise.MuscleGroup.Name,
                CreatedAt = x.CreatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<FavoriteExerciseReadDto> CreateAsync(string userId, FavoriteExerciseCreateDto dto)
    {
        var exerciseExists = await _unitOfWork.Exercises.Query()
            .AnyAsync(x => x.Id == dto.ExerciseId);

        if (!exerciseExists)
        {
            throw new ArgumentException("Vježba ne postoji.");
        }

        var alreadyExists = await _unitOfWork.FavoriteExercises.Query()
            .AnyAsync(x => x.UserId == userId && x.ExerciseId == dto.ExerciseId);

        if (alreadyExists)
        {
            throw new ArgumentException("Vježba je već dodana u favorite.");
        }

        var favorite = new FavoriteExercise
        {
            UserId = userId,
            ExerciseId = dto.ExerciseId
        };

        await _unitOfWork.FavoriteExercises.AddAsync(favorite);
        await _unitOfWork.SaveChangesAsync();

        var created = await GetByIdAsync(favorite.Id, userId, true);

        if (created == null)
        {
            throw new InvalidOperationException("Omiljena vježba nije mogla biti učitana nakon spremanja.");
        }

        return created;
    }

    public async Task<bool> DeleteAsync(int id, string userId, bool isAdmin)
    {
        var favorite = await _unitOfWork.FavoriteExercises.Query()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (favorite == null)
        {
            return false;
        }

        if (!isAdmin && favorite.UserId != userId)
        {
            return false;
        }

        _unitOfWork.FavoriteExercises.Remove(favorite);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}