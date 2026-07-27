using FitnessApp.Api.Data;
using FitnessApp.Api.DTOs;
using FitnessApp.Api.Models;
using FitnessApp.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Api.Services.Implementations;

public class PersonalRecordService : IPersonalRecordService
{
    private readonly IUnitOfWork _unitOfWork;

    public PersonalRecordService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<PersonalRecordReadDto>> GetAllAsync(string userId, bool isAdmin, int? exerciseId = null, DateTime? from = null, DateTime? to = null)
    {
        var query = _unitOfWork.PersonalRecords.Query()
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

        if (exerciseId.HasValue)
        {
            query = query.Where(x => x.ExerciseId == exerciseId.Value);
        }

        if (from.HasValue)
        {
            query = query.Where(x => x.RecordDate >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(x => x.RecordDate <= to.Value);
        }

        return await query
            .OrderByDescending(x => x.RecordDate)
            .Select(x => new PersonalRecordReadDto
            {
                Id = x.Id,
                UserId = x.UserId,
                ExerciseId = x.ExerciseId,
                ExerciseName = x.Exercise.Name,
                ExerciseCategoryName = x.Exercise.ExerciseCategory.Name,
                MuscleGroupName = x.Exercise.MuscleGroup.Name,
                RecordDate = x.RecordDate,
                WeightKg = x.WeightKg,
                Reps = x.Reps,
                Notes = x.Notes,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<PersonalRecordReadDto?> GetByIdAsync(int id, string userId, bool isAdmin)
    {
        var query = _unitOfWork.PersonalRecords.Query()
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
            .Select(x => new PersonalRecordReadDto
            {
                Id = x.Id,
                UserId = x.UserId,
                ExerciseId = x.ExerciseId,
                ExerciseName = x.Exercise.Name,
                ExerciseCategoryName = x.Exercise.ExerciseCategory.Name,
                MuscleGroupName = x.Exercise.MuscleGroup.Name,
                RecordDate = x.RecordDate,
                WeightKg = x.WeightKg,
                Reps = x.Reps,
                Notes = x.Notes,
                CreatedAt = x.CreatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<PersonalRecordReadDto> CreateAsync(string userId, PersonalRecordCreateUpdateDto dto)
    {
        var exerciseExists = await _unitOfWork.Exercises.Query()
            .AnyAsync(x => x.Id == dto.ExerciseId);

        if (!exerciseExists)
        {
            throw new ArgumentException("Vježba ne postoji.");
        }

        var entity = new PersonalRecord
        {
            UserId = userId,
            ExerciseId = dto.ExerciseId,
            RecordDate = dto.RecordDate.Date,
            WeightKg = dto.WeightKg,
            Reps = dto.Reps,
            Notes = dto.Notes
        };

        await _unitOfWork.PersonalRecords.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        var created = await GetByIdAsync(entity.Id, userId, true);

        if (created == null)
        {
            throw new InvalidOperationException("Osobni rekord nije mogao biti učitan nakon spremanja.");
        }

        return created;
    }

    public async Task<bool> UpdateAsync(int id, string userId, bool isAdmin, PersonalRecordCreateUpdateDto dto)
    {
        var entity = await _unitOfWork.PersonalRecords.Query()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null)
        {
            return false;
        }

        if (!isAdmin && entity.UserId != userId)
        {
            return false;
        }

        var exerciseExists = await _unitOfWork.Exercises.Query()
            .AnyAsync(x => x.Id == dto.ExerciseId);

        if (!exerciseExists)
        {
            throw new ArgumentException("Vježba ne postoji.");
        }

        entity.ExerciseId = dto.ExerciseId;
        entity.RecordDate = dto.RecordDate.Date;
        entity.WeightKg = dto.WeightKg;
        entity.Reps = dto.Reps;
        entity.Notes = dto.Notes;

        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id, string userId, bool isAdmin)
    {
        var entity = await _unitOfWork.PersonalRecords.Query()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null)
        {
            return false;
        }

        if (!isAdmin && entity.UserId != userId)
        {
            return false;
        }

        _unitOfWork.PersonalRecords.Remove(entity);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}