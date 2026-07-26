using FitnessApp.Api.Data;
using FitnessApp.Api.DTOs;
using FitnessApp.Api.Models;
using FitnessApp.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Api.Services.Implementations;

public class WorkoutSessionService : IWorkoutSessionService
{
    private readonly IUnitOfWork _unitOfWork;

    public WorkoutSessionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<WorkoutSessionReadDto>> GetAllAsync(string userId, bool isAdmin)
    {
        var query = _unitOfWork.WorkoutSessions.Query()
            .AsNoTracking()
            .Include(x => x.WorkoutPlan)
            .Include(x => x.WorkoutSessionExercises)
                .ThenInclude(x => x.Exercise)
            .Include(x => x.WorkoutSessionExercises)
                .ThenInclude(x => x.CompletedSets)
            .AsQueryable();

        if (!isAdmin)
        {
            query = query.Where(x => x.UserId == userId);
        }

        var sessions = await query
            .OrderByDescending(x => x.ScheduledAt)
            .ToListAsync();

        return sessions.Select(MapToDto).ToList();
    }

    public async Task<WorkoutSessionReadDto?> GetByIdAsync(int id, string userId, bool isAdmin)
    {
        var query = _unitOfWork.WorkoutSessions.Query()
            .AsNoTracking()
            .Include(x => x.WorkoutPlan)
            .Include(x => x.WorkoutSessionExercises)
                .ThenInclude(x => x.Exercise)
            .Include(x => x.WorkoutSessionExercises)
                .ThenInclude(x => x.CompletedSets)
            .Where(x => x.Id == id)
            .AsQueryable();

        if (!isAdmin)
        {
            query = query.Where(x => x.UserId == userId);
        }

        var session = await query.FirstOrDefaultAsync();
        return session == null ? null : MapToDto(session);
    }

    public async Task<WorkoutSessionReadDto> CreateAsync(string userId, bool isAdmin, WorkoutSessionCreateDto dto)
    {
        var planQuery = _unitOfWork.WorkoutPlans.Query()
            .Include(x => x.WorkoutPlanExercises)
                .ThenInclude(x => x.Exercise)
            .AsQueryable();

        if (!isAdmin)
        {
            planQuery = planQuery.Where(x => x.UserId == userId);
        }

        var plan = await planQuery.FirstOrDefaultAsync(x => x.Id == dto.WorkoutPlanId);

        if (plan == null)
        {
            throw new ArgumentException("Plan treninga nije pronađen ili nemaš pristup.");
        }

        if (!plan.WorkoutPlanExercises.Any())
        {
            throw new ArgumentException("Plan treninga nema nijednu vježbu.");
        }

        var session = new WorkoutSession
        {
            UserId = userId,
            WorkoutPlanId = plan.Id,
            ScheduledAt = dto.ScheduledAt,
            Notes = dto.Notes,
            Status = WorkoutSessionStatus.Planned,
            WorkoutSessionExercises = plan.WorkoutPlanExercises
                .OrderBy(x => x.DisplayOrder)
                .Select(x => new WorkoutSessionExercise
                {
                    ExerciseId = x.ExerciseId,
                    DisplayOrder = x.DisplayOrder,
                    PlannedSets = x.Sets,
                    PlannedReps = x.Reps,
                    PlannedRestSeconds = x.RestSeconds
                })
                .ToList()
        };

        await _unitOfWork.WorkoutSessions.AddAsync(session);
        await _unitOfWork.SaveChangesAsync();

        var created = await GetByIdAsync(session.Id, userId, true);
        if (created == null)
        {
            throw new InvalidOperationException("Sesija nije mogla biti učitana nakon spremanja.");
        }

        return created;
    }

    public async Task<bool> UpdateStatusAsync(int id, string userId, bool isAdmin, WorkoutSessionUpdateStatusDto dto)
    {
        var session = await _unitOfWork.WorkoutSessions.Query()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (session == null)
        {
            return false;
        }

        if (!isAdmin && session.UserId != userId)
        {
            return false;
        }

        session.Status = dto.Status;
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> CompleteAsync(int id, string userId, bool isAdmin, WorkoutSessionCompleteDto dto)
    {
        var session = await _unitOfWork.WorkoutSessions.Query()
            .Include(x => x.WorkoutSessionExercises)
                .ThenInclude(x => x.CompletedSets)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (session == null)
        {
            return false;
        }

        if (!isAdmin && session.UserId != userId)
        {
            return false;
        }

        if (dto.Exercises.Count == 0)
        {
            throw new ArgumentException("Moraju biti unesene vježbe i serije.");
        }

        foreach (var exerciseCompletion in dto.Exercises)
        {
            var sessionExercise = session.WorkoutSessionExercises
                .FirstOrDefault(x => x.Id == exerciseCompletion.WorkoutSessionExerciseId);

            if (sessionExercise == null)
            {
                throw new ArgumentException("Jedna od stavki ne pripada ovoj sesiji.");
            }

            foreach (var existingSet in sessionExercise.CompletedSets.ToList())
            {
                _unitOfWork.CompletedSets.Remove(existingSet);
            }

            foreach (var set in exerciseCompletion.CompletedSets.OrderBy(x => x.SetNumber))
            {
                await _unitOfWork.CompletedSets.AddAsync(new CompletedSet
                {
                    WorkoutSessionExerciseId = sessionExercise.Id,
                    SetNumber = set.SetNumber,
                    Reps = set.Reps,
                    WeightKg = set.WeightKg
                });
            }
        }

        session.Status = WorkoutSessionStatus.Completed;
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id, string userId, bool isAdmin)
    {
        var session = await _unitOfWork.WorkoutSessions.Query()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (session == null)
        {
            return false;
        }

        if (!isAdmin && session.UserId != userId)
        {
            return false;
        }

        _unitOfWork.WorkoutSessions.Remove(session);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    private static WorkoutSessionReadDto MapToDto(WorkoutSession session)
    {
        return new WorkoutSessionReadDto
        {
            Id = session.Id,
            UserId = session.UserId,
            WorkoutPlanId = session.WorkoutPlanId,
            WorkoutPlanName = session.WorkoutPlan.Name,
            ScheduledAt = session.ScheduledAt,
            Status = session.Status.ToString(),
            Notes = session.Notes,
            CreatedAt = session.CreatedAt,
            Exercises = session.WorkoutSessionExercises
                .OrderBy(x => x.DisplayOrder)
                .Select(x => new WorkoutSessionExerciseReadDto
                {
                    Id = x.Id,
                    ExerciseId = x.ExerciseId,
                    ExerciseName = x.Exercise.Name,
                    DisplayOrder = x.DisplayOrder,
                    PlannedSets = x.PlannedSets,
                    PlannedReps = x.PlannedReps,
                    PlannedRestSeconds = x.PlannedRestSeconds,
                    CompletedSets = x.CompletedSets
                        .OrderBy(s => s.SetNumber)
                        .Select(s => new CompletedSetReadDto
                        {
                            Id = s.Id,
                            SetNumber = s.SetNumber,
                            Reps = s.Reps,
                            WeightKg = s.WeightKg
                        })
                        .ToList()
                })
                .ToList()
        };
    }
}