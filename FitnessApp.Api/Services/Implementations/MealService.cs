using FitnessApp.Api.Data;
using FitnessApp.Api.DTOs;
using FitnessApp.Api.Models;
using FitnessApp.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Api.Services.Implementations;

public class MealService : IMealService
{
    private readonly IUnitOfWork _unitOfWork;

    public MealService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<MealReadDto>> GetAllAsync()
    {
        return await _unitOfWork.Meals.Query()
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new MealReadDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Calories = x.Calories,
                ProteinGrams = x.ProteinGrams,
                CarbsGrams = x.CarbsGrams,
                FatGrams = x.FatGrams,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<MealReadDto?> GetByIdAsync(int id)
    {
        return await _unitOfWork.Meals.Query()
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new MealReadDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Calories = x.Calories,
                ProteinGrams = x.ProteinGrams,
                CarbsGrams = x.CarbsGrams,
                FatGrams = x.FatGrams,
                CreatedAt = x.CreatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<MealReadDto> CreateAsync(MealCreateUpdateDto dto)
    {
        var meal = new Meal
        {
            Name = dto.Name,
            Description = dto.Description,
            Calories = dto.Calories,
            ProteinGrams = dto.ProteinGrams,
            CarbsGrams = dto.CarbsGrams,
            FatGrams = dto.FatGrams
        };

        await _unitOfWork.Meals.AddAsync(meal);
        await _unitOfWork.SaveChangesAsync();

        var created = await GetByIdAsync(meal.Id);
        if (created == null)
        {
            throw new InvalidOperationException("Obrok nije mogao biti učitan nakon spremanja.");
        }

        return created;
    }

    public async Task<bool> UpdateAsync(int id, MealCreateUpdateDto dto)
    {
        var meal = await _unitOfWork.Meals.Query().FirstOrDefaultAsync(x => x.Id == id);
        if (meal == null)
        {
            return false;
        }

        meal.Name = dto.Name;
        meal.Description = dto.Description;
        meal.Calories = dto.Calories;
        meal.ProteinGrams = dto.ProteinGrams;
        meal.CarbsGrams = dto.CarbsGrams;
        meal.FatGrams = dto.FatGrams;

        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var meal = await _unitOfWork.Meals.Query().FirstOrDefaultAsync(x => x.Id == id);
        if (meal == null)
        {
            return false;
        }

        _unitOfWork.Meals.Remove(meal);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}