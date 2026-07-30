using FitnessApp.Api.Data;
using FitnessApp.Api.DTOs;
using FitnessApp.Api.Models;
using FitnessApp.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Api.Services.Implementations;

public class MealPlanService : IMealPlanService
{
    private readonly IUnitOfWork _unitOfWork;

    public MealPlanService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<MealPlanReadDto>> GetAllAsync(string userId, bool isAdmin)
    {
        var query = _unitOfWork.MealPlans.Query()
            .AsNoTracking()
            .Include(x => x.MealPlanMeals)
                .ThenInclude(x => x.Meal)
            .AsQueryable();

        if (!isAdmin)
        {
            query = query.Where(x => x.UserId == userId);
        }

        var plans = await query
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return plans.Select(MapToDto).ToList();
    }

    public async Task<MealPlanReadDto?> GetByIdAsync(int id, string userId, bool isAdmin)
    {
        var query = _unitOfWork.MealPlans.Query()
            .AsNoTracking()
            .Include(x => x.MealPlanMeals)
                .ThenInclude(x => x.Meal)
            .Where(x => x.Id == id)
            .AsQueryable();

        if (!isAdmin)
        {
            query = query.Where(x => x.UserId == userId);
        }

        var plan = await query.FirstOrDefaultAsync();
        return plan == null ? null : MapToDto(plan);
    }

    public async Task<MealPlanReadDto> CreateAsync(string userId, MealPlanCreateUpdateDto dto)
    {
        if (dto.Meals.Count == 0)
        {
            throw new ArgumentException("Plan prehrane mora sadržavati barem jedan obrok.");
        }

        if (dto.Meals.Any(x => x.DisplayOrder <= 0))
        {
            throw new ArgumentException("Redoslijed obroka mora biti veći od 0.");
        }

        if (dto.Meals.Any(x => x.PortionMultiplier <= 0))
        {
            throw new ArgumentException("Količina obroka mora biti veća od 0.");
        }

        var mealIds = dto.Meals.Select(x => x.MealId).Distinct().ToList();
        var existingMeals = await _unitOfWork.Meals.Query()
            .Where(x => mealIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id);

        if (existingMeals.Count != mealIds.Count)
        {
            throw new ArgumentException("Jedan ili više obroka ne postoji.");
        }

        var mealPlan = new MealPlan
        {
            UserId = userId,
            Name = dto.Name,
            Description = dto.Description,
            DailyCaloriesTarget = dto.DailyCaloriesTarget,
            MealPlanMeals = dto.Meals
                .OrderBy(x => x.DisplayOrder)
                .Select(x => new MealPlanMeal
                {
                    MealId = x.MealId,
                    MealSlot = x.MealSlot.Trim(),
                    DisplayOrder = x.DisplayOrder,
                    PortionMultiplier = x.PortionMultiplier,
                    Notes = x.Notes
                })
                .ToList()
        };

        if (dto.Meals.Select(x => x.MealId).Distinct().Count() != dto.Meals.Count)
        {
            throw new ArgumentException("Isti obrok ne smije biti dodan više puta u plan.");
        }

        if (dto.Meals.Select(x => x.DisplayOrder).Distinct().Count() != dto.Meals.Count)
        {
            throw new ArgumentException("Redoslijed obroka mora biti jedinstven.");
        }

        await _unitOfWork.MealPlans.AddAsync(mealPlan);
        await _unitOfWork.SaveChangesAsync();

        var created = await GetByIdAsync(mealPlan.Id, userId, true);
        if (created == null)
        {
            throw new InvalidOperationException("Plan prehrane nije mogao biti učitan nakon spremanja.");
        }

        return created;
    }

    public async Task<bool> UpdateAsync(int id, string userId, bool isAdmin, MealPlanCreateUpdateDto dto)
    {
        var mealPlan = await _unitOfWork.MealPlans.Query()
            .Include(x => x.MealPlanMeals)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (mealPlan == null)
        {
            return false;
        }

        if (!isAdmin && mealPlan.UserId != userId)
        {
            return false;
        }

        if (dto.Meals.Count == 0)
        {
            throw new ArgumentException("Plan prehrane mora sadržavati barem jedan obrok.");
        }

        if (dto.Meals.Any(x => x.DisplayOrder <= 0))
        {
            throw new ArgumentException("Redoslijed obroka mora biti veći od 0.");
        }

        if (dto.Meals.Any(x => x.PortionMultiplier <= 0))
        {
            throw new ArgumentException("Količina obroka mora biti veća od 0.");
        }

        var mealIds = dto.Meals.Select(x => x.MealId).Distinct().ToList();
        var existingMeals = await _unitOfWork.Meals.Query()
            .Where(x => mealIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id);

        if (existingMeals.Count != mealIds.Count)
        {
            throw new ArgumentException("Jedan ili više obroka ne postoji.");
        }

        mealPlan.Name = dto.Name;
        mealPlan.Description = dto.Description;
        mealPlan.DailyCaloriesTarget = dto.DailyCaloriesTarget;

        foreach (var item in mealPlan.MealPlanMeals.ToList())
        {
            _unitOfWork.MealPlanMeals.Remove(item);
        }

        foreach (var item in dto.Meals.OrderBy(x => x.DisplayOrder))
        {
            await _unitOfWork.MealPlanMeals.AddAsync(new MealPlanMeal
            {
                MealPlanId = mealPlan.Id,
                MealId = item.MealId,
                MealSlot = item.MealSlot.Trim(),
                DisplayOrder = item.DisplayOrder,
                PortionMultiplier = item.PortionMultiplier,
                Notes = item.Notes
            });
        }

        if (dto.Meals.Select(x => x.MealId).Distinct().Count() != dto.Meals.Count)
        {
            throw new ArgumentException("Isti obrok ne smije biti dodan više puta u plan.");
        }

        if (dto.Meals.Select(x => x.DisplayOrder).Distinct().Count() != dto.Meals.Count)
        {
            throw new ArgumentException("Redoslijed obroka mora biti jedinstven.");
        }

        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id, string userId, bool isAdmin)
    {
        var mealPlan = await _unitOfWork.MealPlans.Query()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (mealPlan == null)
        {
            return false;
        }

        if (!isAdmin && mealPlan.UserId != userId)
        {
            return false;
        }

        _unitOfWork.MealPlans.Remove(mealPlan);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    private static MealPlanReadDto MapToDto(MealPlan mealPlan)
    {
        var meals = mealPlan.MealPlanMeals
            .OrderBy(x => x.DisplayOrder)
            .Select(x => new MealPlanMealReadDto
            {
                Id = x.Id,
                MealId = x.MealId,
                MealName = x.Meal.Name,
                MealCalories = x.Meal.Calories,
                ProteinGrams = x.Meal.ProteinGrams,
                CarbsGrams = x.Meal.CarbsGrams,
                FatGrams = x.Meal.FatGrams,
                MealSlot = x.MealSlot,
                DisplayOrder = x.DisplayOrder,
                PortionMultiplier = x.PortionMultiplier,
                Notes = x.Notes
            })
            .ToList();

        return new MealPlanReadDto
        {
            Id = mealPlan.Id,
            UserId = mealPlan.UserId,
            Name = mealPlan.Name,
            Description = mealPlan.Description,
            DailyCaloriesTarget = mealPlan.DailyCaloriesTarget,
            TotalCalories = meals.Sum(x => x.MealCalories * x.PortionMultiplier),
            CreatedAt = mealPlan.CreatedAt,
            Meals = meals
        };
    }
}