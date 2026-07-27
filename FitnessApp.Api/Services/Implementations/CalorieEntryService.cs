using FitnessApp.Api.Data;
using FitnessApp.Api.DTOs;
using FitnessApp.Api.Models;
using FitnessApp.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Api.Services.Implementations;

public class CalorieEntryService : ICalorieEntryService
{
    private readonly IUnitOfWork _unitOfWork;

    public CalorieEntryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<CalorieEntryReadDto>> GetAllAsync(string userId, bool isAdmin, DateTime? from = null, DateTime? to = null)
    {
        var query = _unitOfWork.CalorieEntries.Query()
            .AsNoTracking()
            .AsQueryable();

        if (!isAdmin)
        {
            query = query.Where(x => x.UserId == userId);
        }

        if (from.HasValue)
        {
            query = query.Where(x => x.EntryDate >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(x => x.EntryDate <= to.Value);
        }

        return await query
            .OrderByDescending(x => x.EntryDate)
            .Select(x => new CalorieEntryReadDto
            {
                Id = x.Id,
                UserId = x.UserId,
                EntryDate = x.EntryDate,
                Calories = x.Calories,
                Notes = x.Notes,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<CalorieEntryReadDto?> GetByIdAsync(int id, string userId, bool isAdmin)
    {
        var query = _unitOfWork.CalorieEntries.Query()
            .AsNoTracking()
            .Where(x => x.Id == id)
            .AsQueryable();

        if (!isAdmin)
        {
            query = query.Where(x => x.UserId == userId);
        }

        return await query
            .Select(x => new CalorieEntryReadDto
            {
                Id = x.Id,
                UserId = x.UserId,
                EntryDate = x.EntryDate,
                Calories = x.Calories,
                Notes = x.Notes,
                CreatedAt = x.CreatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<CalorieEntryReadDto> CreateAsync(string userId, CalorieEntryCreateUpdateDto dto)
    {
        var entryDate = dto.EntryDate.Date;

        var existing = await _unitOfWork.CalorieEntries.Query()
            .FirstOrDefaultAsync(x => x.UserId == userId && x.EntryDate == entryDate);

        if (existing != null)
        {
            throw new ArgumentException("Unos kalorija za taj datum već postoji.");
        }

        var entity = new CalorieEntry
        {
            UserId = userId,
            EntryDate = entryDate,
            Calories = dto.Calories,
            Notes = dto.Notes
        };

        await _unitOfWork.CalorieEntries.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return new CalorieEntryReadDto
        {
            Id = entity.Id,
            UserId = entity.UserId,
            EntryDate = entity.EntryDate,
            Calories = entity.Calories,
            Notes = entity.Notes,
            CreatedAt = entity.CreatedAt
        };
    }

    public async Task<bool> UpdateAsync(int id, string userId, bool isAdmin, CalorieEntryCreateUpdateDto dto)
    {
        var entity = await _unitOfWork.CalorieEntries.Query()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null)
        {
            return false;
        }

        if (!isAdmin && entity.UserId != userId)
        {
            return false;
        }

        var targetDate = dto.EntryDate.Date;

        var duplicate = await _unitOfWork.CalorieEntries.Query()
            .AnyAsync(x => x.UserId == entity.UserId && x.EntryDate == targetDate && x.Id != id);

        if (duplicate)
        {
            throw new ArgumentException("Unos kalorija za taj datum već postoji.");
        }

        entity.EntryDate = targetDate;
        entity.Calories = dto.Calories;
        entity.Notes = dto.Notes;

        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id, string userId, bool isAdmin)
    {
        var entity = await _unitOfWork.CalorieEntries.Query()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null)
        {
            return false;
        }

        if (!isAdmin && entity.UserId != userId)
        {
            return false;
        }

        _unitOfWork.CalorieEntries.Remove(entity);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}