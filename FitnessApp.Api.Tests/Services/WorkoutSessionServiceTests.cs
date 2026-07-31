using FitnessApp.Api.Data;
using FitnessApp.Api.DTOs;
using FitnessApp.Api.Models;
using FitnessApp.Api.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace FitnessApp.Api.Tests.Services;

[TestFixture]
public class WorkoutSessionServiceTests
{
    private const string CurrentUserId = "user-1";
    private const string OtherUserId = "user-2";

    private ApplicationDbContext _context = null!;
    private IUnitOfWork _unitOfWork = null!;
    private WorkoutSessionService _service = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);

        SeedBaseData();
        SeedWorkoutPlans();
        SeedWorkoutSessions();
        SeedWorkoutSessionExercises();
        SeedWorkoutPlanExercises();

        _context.SaveChanges();

        _unitOfWork = new UnitOfWork(_context);
        _service = new WorkoutSessionService(_unitOfWork);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public async Task GetAllAsync_ReturnsOnlyCurrentUsersSessions_WhenNotAdmin()
    {
        var result = (await _service.GetAllAsync(CurrentUserId, isAdmin: false)).ToList();

        Assert.That(result.Count, Is.EqualTo(3));
        Assert.That(result.All(x => x.UserId == CurrentUserId), Is.True);
        Assert.That(result[0].ScheduledAt, Is.EqualTo(new DateTime(2026, 2, 2)));
    }

    [Test]
    public async Task GetAllAsync_ReturnsAllSessions_WhenAdmin()
    {
        var result = (await _service.GetAllAsync(CurrentUserId, isAdmin: true)).ToList();

        Assert.That(result.Count, Is.EqualTo(4));
        Assert.That(result.Any(x => x.UserId == OtherUserId), Is.True);
    }

    [Test]
    public async Task GetByIdAsync_ReturnsNull_WhenSessionDoesNotExist()
    {
        var result = await _service.GetByIdAsync(999, CurrentUserId, isAdmin: false);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetByIdAsync_ReturnsNull_WhenSessionBelongsToAnotherUser_AndNotAdmin()
    {
        var result = await _service.GetByIdAsync(4, CurrentUserId, isAdmin: false);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetByIdAsync_ReturnsSession_WhenAdmin()
    {
        var result = await _service.GetByIdAsync(4, CurrentUserId, isAdmin: true);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.UserId, Is.EqualTo(OtherUserId));
        Assert.That(result.WorkoutPlanName, Is.EqualTo("Plan 2"));
    }

    [Test]
    public async Task CreateAsync_ThrowsArgumentException_WhenScheduledAtIsDefault()
    {
        var dto = new WorkoutSessionCreateDto
        {
            WorkoutPlanId = 1,
            ScheduledAt = default,
            Notes = "Test"
        };

        var ex = await Task.Run(() => Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.CreateAsync(CurrentUserId, isAdmin: false, dto)));

        Assert.That(ex!.Message, Is.EqualTo("Vrijeme sesije mora biti zadano."));
    }

    [Test]
    public async Task CreateAsync_ThrowsArgumentException_WhenPlanDoesNotExistOrNoAccess()
    {
        var dto = new WorkoutSessionCreateDto
        {
            WorkoutPlanId = 2,
            ScheduledAt = new DateTime(2026, 3, 1),
            Notes = "Test"
        };

        var ex = await Task.Run(() => Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.CreateAsync(CurrentUserId, false, dto)));

        Assert.That(ex!.Message, Is.EqualTo("Plan treninga nije pronađen ili nemaš pristup."));
    }

    [Test]
    public async Task CreateAsync_AddsSessionAndCopiesExercises_WhenValid()
    {
        var dto = new WorkoutSessionCreateDto
        {
            WorkoutPlanId = 1,
            ScheduledAt = new DateTime(2026, 3, 1, 18, 0, 0),
            Notes = "Večernji trening"
        };

        var result = await _service.CreateAsync(CurrentUserId, isAdmin: false, dto);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.UserId, Is.EqualTo(CurrentUserId));
        Assert.That(result.WorkoutPlanId, Is.EqualTo(1));
        Assert.That(result.Status, Is.EqualTo(WorkoutSessionStatus.Planned.ToString()));
        Assert.That(result.Exercises.Count, Is.EqualTo(2));
        Assert.That(result.Exercises[0].ExerciseName, Is.EqualTo("Bench Press"));
        Assert.That(result.Exercises[0].PlannedSets, Is.EqualTo(3));

        _context.ChangeTracker.Clear();

        var sessionExerciseCount = await _context.WorkoutSessionExercises
            .CountAsync(x => x.WorkoutSessionId == result.Id);

        Assert.That(sessionExerciseCount, Is.EqualTo(2));
    }

    [Test]
    public async Task UpdateStatusAsync_ReturnsFalse_WhenSessionDoesNotExist()
    {
        var dto = new WorkoutSessionUpdateStatusDto
        {
            Status = WorkoutSessionStatus.Completed
        };

        var result = await _service.UpdateStatusAsync(999, CurrentUserId, isAdmin: false, dto);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task UpdateStatusAsync_ReturnsFalse_WhenSessionBelongsToAnotherUser_AndNotAdmin()
    {
        var dto = new WorkoutSessionUpdateStatusDto
        {
            Status = WorkoutSessionStatus.Completed
        };

        var result = await _service.UpdateStatusAsync(4, CurrentUserId, isAdmin: false, dto);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task UpdateStatusAsync_UpdatesStatus_WhenValid()
    {
        var dto = new WorkoutSessionUpdateStatusDto
        {
            Status = WorkoutSessionStatus.Skipped
        };

        var result = await _service.UpdateStatusAsync(1, CurrentUserId, isAdmin: false, dto);

        Assert.That(result, Is.True);

        var updatedSession = await _context.WorkoutSessions.FirstAsync(x => x.Id == 1);
        Assert.That(updatedSession.Status, Is.EqualTo(WorkoutSessionStatus.Skipped));
    }

    [Test]
    public async Task CompleteAsync_ThrowsArgumentException_WhenNoExercisesProvided()
    {
        var dto = new WorkoutSessionCompleteDto();

        var ex = await Task.Run(() => Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.CompleteAsync(1, CurrentUserId, isAdmin: false, dto)));

        Assert.That(ex!.Message, Is.EqualTo("Moraju biti unesene vježbe i serije."));
    }

    [Test]
    public async Task CompleteAsync_ThrowsArgumentException_WhenSessionExerciseDoesNotBelongToSession()
    {
        var dto = new WorkoutSessionCompleteDto
        {
            Exercises = new List<WorkoutSessionExerciseCompletionDto>
        {
            new WorkoutSessionExerciseCompletionDto
            {
                WorkoutSessionExerciseId = 999,
                CompletedSets = new List<CompletedSetUpsertDto>
                {
                    new CompletedSetUpsertDto
                    {
                        SetNumber = 1,
                        Reps = 10,
                        WeightKg = 60m
                    }
                }
            }
        }
        };

        var ex = await Task.Run(() => Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.CompleteAsync(1, CurrentUserId, false, dto)));

        Assert.That(ex!.Message, Is.EqualTo("Jedna od stavki ne pripada ovoj sesiji."));
    }

    [Test]
    public async Task CompleteAsync_UpdatesStatusAndAddsCompletedSets_WhenValid()
    {
        var dto = new WorkoutSessionCompleteDto
        {
            Exercises = new List<WorkoutSessionExerciseCompletionDto>
            {
                new WorkoutSessionExerciseCompletionDto
                {
                    WorkoutSessionExerciseId = 2,
                    CompletedSets = new List<CompletedSetUpsertDto>
                    {
                        new CompletedSetUpsertDto { SetNumber = 1, Reps = 10, WeightKg = 60m },
                        new CompletedSetUpsertDto { SetNumber = 2, Reps = 8, WeightKg = 60m }
                    }
                },
                new WorkoutSessionExerciseCompletionDto
                {
                    WorkoutSessionExerciseId = 3,
                    CompletedSets = new List<CompletedSetUpsertDto>
                    {
                        new CompletedSetUpsertDto { SetNumber = 1, Reps = 5, WeightKg = 100m }
                    }
                }
            }
        };

        var result = await _service.CompleteAsync(3, CurrentUserId, isAdmin: false, dto);

        Assert.That(result, Is.True);

        var completedSession = await _context.WorkoutSessions
            .Include(x => x.WorkoutSessionExercises)
                .ThenInclude(x => x.CompletedSets)
            .FirstAsync(x => x.Id == 3);

        Assert.That(completedSession.Status, Is.EqualTo(WorkoutSessionStatus.Completed));
        Assert.That(completedSession.WorkoutSessionExercises.Count, Is.EqualTo(2));
        Assert.That(completedSession.WorkoutSessionExercises.First(x => x.Id == 2).CompletedSets.Count, Is.EqualTo(2));
        Assert.That(completedSession.WorkoutSessionExercises.First(x => x.Id == 3).CompletedSets.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task DeleteAsync_ReturnsFalse_WhenSessionDoesNotExist()
    {
        var result = await _service.DeleteAsync(999, CurrentUserId, isAdmin: false);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task DeleteAsync_ReturnsFalse_WhenSessionBelongsToAnotherUser_AndNotAdmin()
    {
        var result = await _service.DeleteAsync(4, CurrentUserId, isAdmin: false);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task DeleteAsync_RemovesSession_WhenValid()
    {
        var result = await _service.DeleteAsync(1, CurrentUserId, isAdmin: false);

        Assert.That(result, Is.True);

        var deleted = await _context.WorkoutSessions.FirstOrDefaultAsync(x => x.Id == 1);
        Assert.That(deleted, Is.Null);
    }

    private void SeedBaseData()
    {
        _context.ExerciseCategories.AddRange(
            new ExerciseCategory { Id = 1, Name = "Snaga" },
            new ExerciseCategory { Id = 2, Name = "Kardio" });

        _context.MuscleGroups.AddRange(
            new MuscleGroup { Id = 1, Name = "Prsa" },
            new MuscleGroup { Id = 2, Name = "Noge" });

        _context.Exercises.AddRange(
            new Exercise
            {
                Id = 1,
                Name = "Bench Press",
                Description = "Vježba za prsa",
                YoutubeUrl = "https://youtube.com/bench",
                ExerciseCategoryId = 1,
                MuscleGroupId = 1
            },
            new Exercise
            {
                Id = 2,
                Name = "Squat",
                Description = "Vježba za noge",
                YoutubeUrl = "https://youtube.com/squat",
                ExerciseCategoryId = 1,
                MuscleGroupId = 2
            });
    }

    private void SeedWorkoutPlans()
    {
        _context.WorkoutPlans.AddRange(
            new WorkoutPlan
            {
                Id = 1,
                UserId = CurrentUserId,
                Name = "Plan 1",
                Description = "Moj plan"
            },
            new WorkoutPlan
            {
                Id = 2,
                UserId = OtherUserId,
                Name = "Plan 2",
                Description = "Drugi plan"
            });
    }

    private void SeedWorkoutSessions()
    {
        _context.WorkoutSessions.AddRange(
            new WorkoutSession
            {
                Id = 1,
                UserId = CurrentUserId,
                WorkoutPlanId = 1,
                ScheduledAt = new DateTime(2026, 1, 5),
                Status = WorkoutSessionStatus.Completed,
                Notes = "Prvi trening"
            },
            new WorkoutSession
            {
                Id = 2,
                UserId = CurrentUserId,
                WorkoutPlanId = 1,
                ScheduledAt = new DateTime(2026, 1, 20),
                Status = WorkoutSessionStatus.Planned,
                Notes = "Planirani trening"
            },
            new WorkoutSession
            {
                Id = 3,
                UserId = CurrentUserId,
                WorkoutPlanId = 1,
                ScheduledAt = new DateTime(2026, 2, 2),
                Status = WorkoutSessionStatus.Completed,
                Notes = "Drugi trening"
            },
            new WorkoutSession
            {
                Id = 4,
                UserId = OtherUserId,
                WorkoutPlanId = 2,
                ScheduledAt = new DateTime(2026, 3, 1),
                Status = WorkoutSessionStatus.Completed,
                Notes = "Tuđi trening"
            });
    }

    private void SeedWorkoutSessionExercises()
    {
        _context.WorkoutSessionExercises.AddRange(
            new WorkoutSessionExercise
            {
                Id = 1,
                WorkoutSessionId = 1,
                ExerciseId = 1,
                DisplayOrder = 1,
                PlannedSets = 3,
                PlannedReps = 10,
                PlannedRestSeconds = 90
            },
            new WorkoutSessionExercise
            {
                Id = 2,
                WorkoutSessionId = 3,
                ExerciseId = 1,
                DisplayOrder = 1,
                PlannedSets = 3,
                PlannedReps = 8,
                PlannedRestSeconds = 90
            },
            new WorkoutSessionExercise
            {
                Id = 3,
                WorkoutSessionId = 3,
                ExerciseId = 2,
                DisplayOrder = 2,
                PlannedSets = 4,
                PlannedReps = 6,
                PlannedRestSeconds = 120
            },
            new WorkoutSessionExercise
            {
                Id = 4,
                WorkoutSessionId = 4,
                ExerciseId = 1,
                DisplayOrder = 1,
                PlannedSets = 5,
                PlannedReps = 5,
                PlannedRestSeconds = 120
            });
    }

    private void SeedWorkoutPlanExercises()
    {
        _context.WorkoutPlanExercises.AddRange(
            new WorkoutPlanExercise
            {
                WorkoutPlanId = 1,
                ExerciseId = 1,
                DisplayOrder = 1,
                Sets = 3,
                Reps = 10,
                RestSeconds = 90
            },
            new WorkoutPlanExercise
            {
                WorkoutPlanId = 1,
                ExerciseId = 2,
                DisplayOrder = 2,
                Sets = 4,
                Reps = 8,
                RestSeconds = 60
            },
            new WorkoutPlanExercise
            {
                WorkoutPlanId = 2,
                ExerciseId = 1,
                DisplayOrder = 1,
                Sets = 5,
                Reps = 5,
                RestSeconds = 120
            });
    }
}