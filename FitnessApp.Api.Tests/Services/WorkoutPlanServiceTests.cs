using FitnessApp.Api.Data;
using FitnessApp.Api.DTOs;
using FitnessApp.Api.Models;
using FitnessApp.Api.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace FitnessApp.Api.Tests.Services;

[TestFixture]
public class WorkoutPlanServiceTests
{
    private const string CurrentUserId = "user-1";
    private const string OtherUserId = "user-2";

    private ApplicationDbContext _context = null!;
    private IUnitOfWork _unitOfWork = null!;
    private WorkoutPlanService _service = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);

        SeedBaseData();

        _context.SaveChanges();

        _unitOfWork = new UnitOfWork(_context);
        _service = new WorkoutPlanService(_unitOfWork);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public async Task GetAllAsync_ReturnsOnlyCurrentUsersPlans_WhenNotAdmin()
    {
        var result = (await _service.GetAllAsync(CurrentUserId, isAdmin: false)).ToList();

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].Name, Is.EqualTo("Plan 1"));
        Assert.That(result[0].UserId, Is.EqualTo(CurrentUserId));
        Assert.That(result[0].Exercises.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task GetAllAsync_ReturnsAllPlans_WhenAdmin()
    {
        var result = (await _service.GetAllAsync(CurrentUserId, isAdmin: true)).ToList();

        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.Any(x => x.Name == "Plan 1"), Is.True);
        Assert.That(result.Any(x => x.Name == "Plan 2"), Is.True);
    }

    [Test]
    public async Task GetByIdAsync_ReturnsNull_WhenPlanDoesNotExist()
    {
        var result = await _service.GetByIdAsync(999, CurrentUserId, isAdmin: false);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetByIdAsync_ReturnsNull_WhenPlanBelongsToAnotherUser_AndNotAdmin()
    {
        var result = await _service.GetByIdAsync(2, CurrentUserId, isAdmin: false);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetByIdAsync_ReturnsPlan_WhenAdmin()
    {
        var result = await _service.GetByIdAsync(2, CurrentUserId, isAdmin: true);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Name, Is.EqualTo("Plan 2"));
    }

    [Test]
    public async Task CreateAsync_ThrowsArgumentException_WhenNoExercisesProvided()
    {
        var dto = new WorkoutPlanCreateUpdateDto
        {
            Name = "Novi plan",
            Description = "Opis",
            Exercises = new List<WorkoutPlanExerciseCreateUpdateDto>()
        };

        await Task.CompletedTask;

        var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _service.CreateAsync(CurrentUserId, dto));

        Assert.That(ex!.Message, Is.EqualTo("Plan treninga mora sadržavati barem jednu vježbu."));
    }

    [Test]
    public async Task CreateAsync_ThrowsArgumentException_WhenExerciseDoesNotExist()
    {
        var dto = new WorkoutPlanCreateUpdateDto
        {
            Name = "Novi plan",
            Description = "Opis",
            Exercises = new List<WorkoutPlanExerciseCreateUpdateDto>
           {
               new WorkoutPlanExerciseCreateUpdateDto
               {
                   ExerciseId = 999,
                   DisplayOrder = 1,
                   Sets = 3,
                   Reps = 10,
                   RestSeconds = 90
               }
           }
        };

        await Task.Yield();

        var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _service.CreateAsync(CurrentUserId, dto));

        Assert.That(ex!.Message, Is.EqualTo("Jedna ili više vježbi ne postoji."));
    }

    [Test]
    public async Task CreateAsync_ThrowsArgumentException_WhenSameExerciseAddedTwice()
    {
        var dto = new WorkoutPlanCreateUpdateDto
        {
            Name = "Novi plan",
            Description = "Opis",
            Exercises = new List<WorkoutPlanExerciseCreateUpdateDto>
           {
               new WorkoutPlanExerciseCreateUpdateDto
               {
                   ExerciseId = 1,
                   DisplayOrder = 1,
                   Sets = 3,
                   Reps = 10,
                   RestSeconds = 90
               },
               new WorkoutPlanExerciseCreateUpdateDto
               {
                   ExerciseId = 1,
                   DisplayOrder = 2,
                   Sets = 4,
                   Reps = 8,
                   RestSeconds = 60
               }
           }
        };

        await Task.Yield();

        var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _service.CreateAsync(CurrentUserId, dto));

        Assert.That(ex!.Message, Is.EqualTo("Ista vježba ne smije biti dodana više puta u plan."));
    }

    [Test]
    public async Task CreateAsync_ThrowsArgumentException_WhenDisplayOrderIsDuplicated()
    {
        var dto = new WorkoutPlanCreateUpdateDto
        {
            Name = "Novi plan",
            Description = "Opis",
            Exercises = new List<WorkoutPlanExerciseCreateUpdateDto>
           {
               new WorkoutPlanExerciseCreateUpdateDto
               {
                   ExerciseId = 1,
                   DisplayOrder = 1,
                   Sets = 3,
                   Reps = 10,
                   RestSeconds = 90
               },
               new WorkoutPlanExerciseCreateUpdateDto
               {
                   ExerciseId = 2,
                   DisplayOrder = 1,
                   Sets = 4,
                   Reps = 8,
                   RestSeconds = 60
               }
           }
        };

        await Task.Yield();

        var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _service.CreateAsync(CurrentUserId, dto));

        Assert.That(ex!.Message, Is.EqualTo("Redoslijed vježbi mora biti jedinstven."));
    }

    [Test]
    public async Task CreateAsync_AddsWorkoutPlan_WhenDtoIsValid()
    {
        var dto = new WorkoutPlanCreateUpdateDto
        {
            Name = "Novi plan",
            Description = "Opis plana",
            Exercises = new List<WorkoutPlanExerciseCreateUpdateDto>
            {
                new WorkoutPlanExerciseCreateUpdateDto
                {
                    ExerciseId = 1,
                    DisplayOrder = 1,
                    Sets = 3,
                    Reps = 10,
                    RestSeconds = 90
                },
                new WorkoutPlanExerciseCreateUpdateDto
                {
                    ExerciseId = 2,
                    DisplayOrder = 2,
                    Sets = 4,
                    Reps = 8,
                    RestSeconds = 60
                }
            }
        };

        var result = await _service.CreateAsync(CurrentUserId, dto);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Novi plan"));
        Assert.That(result.UserId, Is.EqualTo(CurrentUserId));
        Assert.That(result.Exercises.Count, Is.EqualTo(2));

        var dbPlan = await _context.WorkoutPlans
            .Include(x => x.WorkoutPlanExercises)
            .FirstOrDefaultAsync(x => x.Name == "Novi plan");

        Assert.That(dbPlan, Is.Not.Null);
        Assert.That(dbPlan!.WorkoutPlanExercises.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task UpdateAsync_ReturnsFalse_WhenPlanDoesNotExist()
    {
        var dto = new WorkoutPlanCreateUpdateDto
        {
            Name = "Updated plan",
            Description = "Updated description",
            Exercises = new List<WorkoutPlanExerciseCreateUpdateDto>
            {
                new WorkoutPlanExerciseCreateUpdateDto
                {
                    ExerciseId = 1,
                    DisplayOrder = 1,
                    Sets = 3,
                    Reps = 10,
                    RestSeconds = 90
                }
            }
        };

        var result = await _service.UpdateAsync(999, CurrentUserId, isAdmin: false, dto);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task UpdateAsync_ReturnsFalse_WhenPlanBelongsToAnotherUser_AndNotAdmin()
    {
        var dto = new WorkoutPlanCreateUpdateDto
        {
            Name = "Updated plan",
            Description = "Updated description",
            Exercises = new List<WorkoutPlanExerciseCreateUpdateDto>
            {
                new WorkoutPlanExerciseCreateUpdateDto
                {
                    ExerciseId = 1,
                    DisplayOrder = 1,
                    Sets = 3,
                    Reps = 10,
                    RestSeconds = 90
                }
            }
        };

        var result = await _service.UpdateAsync(2, CurrentUserId, isAdmin: false, dto);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task UpdateAsync_UpdatesWorkoutPlan_WhenDtoIsValid()
    {
        var dto = new WorkoutPlanCreateUpdateDto
        {
            Name = "Updated plan",
            Description = "Updated description",
            Exercises = new List<WorkoutPlanExerciseCreateUpdateDto>
            {
                new WorkoutPlanExerciseCreateUpdateDto
                {
                    ExerciseId = 2,
                    DisplayOrder = 1,
                    Sets = 5,
                    Reps = 5,
                    RestSeconds = 120
                }
            }
        };

        var result = await _service.UpdateAsync(1, CurrentUserId, isAdmin: false, dto);

        Assert.That(result, Is.True);

        var updatedPlan = await _context.WorkoutPlans
            .Include(x => x.WorkoutPlanExercises)
            .ThenInclude(x => x.Exercise)
            .FirstAsync(x => x.Id == 1);

        Assert.That(updatedPlan.Name, Is.EqualTo("Updated plan"));
        Assert.That(updatedPlan.Description, Is.EqualTo("Updated description"));
        Assert.That(updatedPlan.WorkoutPlanExercises.Count, Is.EqualTo(1));
        Assert.That(updatedPlan.WorkoutPlanExercises.First().ExerciseId, Is.EqualTo(2));
    }

    [Test]
    public async Task DeleteAsync_ReturnsFalse_WhenPlanDoesNotExist()
    {
        var result = await _service.DeleteAsync(999, CurrentUserId, isAdmin: false);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task DeleteAsync_ReturnsFalse_WhenPlanBelongsToAnotherUser_AndNotAdmin()
    {
        var result = await _service.DeleteAsync(2, CurrentUserId, isAdmin: false);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task DeleteAsync_RemovesWorkoutPlan_WhenExists()
    {
        var result = await _service.DeleteAsync(1, CurrentUserId, isAdmin: false);

        Assert.That(result, Is.True);

        var deleted = await _context.WorkoutPlans.FirstOrDefaultAsync(x => x.Id == 1);
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