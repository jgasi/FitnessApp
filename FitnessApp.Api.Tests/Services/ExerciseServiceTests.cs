using FitnessApp.Api.Data;
using FitnessApp.Api.DTOs;
using FitnessApp.Api.Models;
using FitnessApp.Api.Services.Implementations;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Api.Tests.Services;

[TestFixture]
public class ExerciseServiceTests
{
    private ApplicationDbContext _context = null!;
    private IUnitOfWork _unitOfWork = null!;
    private ExerciseService _service = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);

        _context.ExerciseCategories.AddRange(
            new ExerciseCategory { Id = 1, Name = "Snaga" },
            new ExerciseCategory { Id = 2, Name = "Kardio" }
        );

        _context.MuscleGroups.AddRange(
            new MuscleGroup { Id = 1, Name = "Prsa" },
            new MuscleGroup { Id = 2, Name = "Noge" }
        );

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
            }
        );

        _context.SaveChanges();

        _unitOfWork = new UnitOfWork(_context);
        _service = new ExerciseService(_unitOfWork);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public async Task GetAllAsync_ReturnsFilteredExercises()
    {
        var result = await _service.GetAllAsync(
            search: "Bench",
            exerciseCategoryId: 1,
            muscleGroupId: 1);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(1));

        var exercise = result.First();
        Assert.That(exercise.Name, Is.EqualTo("Bench Press"));
        Assert.That(exercise.ExerciseCategoryName, Is.EqualTo("Snaga"));
        Assert.That(exercise.MuscleGroupName, Is.EqualTo("Prsa"));
    }

    [Test]
    public async Task GetByIdAsync_ReturnsExercise_WhenExists()
    {
        var result = await _service.GetByIdAsync(1);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Name, Is.EqualTo("Bench Press"));
        Assert.That(result.ExerciseCategoryName, Is.EqualTo("Snaga"));
        Assert.That(result.MuscleGroupName, Is.EqualTo("Prsa"));
    }

    [Test]
    public async Task GetByIdAsync_ReturnsNull_WhenDoesNotExist()
    {
        var result = await _service.GetByIdAsync(999);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task CreateAsync_AddsExercise_WhenDtoIsValid()
    {
        var dto = new ExerciseCreateUpdateDto
        {
            Name = "Deadlift",
            Description = "Vježba za cijelo tijelo",
            YoutubeUrl = "https://youtube.com/deadlift",
            ExerciseCategoryId = 1,
            MuscleGroupId = 2
        };

        var result = await _service.CreateAsync(dto);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Deadlift"));
        Assert.That(result.ExerciseCategoryName, Is.EqualTo("Snaga"));
        Assert.That(result.MuscleGroupName, Is.EqualTo("Noge"));

        var dbExercise = await _context.Exercises.FirstOrDefaultAsync(x => x.Name == "Deadlift");
        Assert.That(dbExercise, Is.Not.Null);
    }

    [Test]
    public async Task CreateAsync_ThrowsArgumentException_WhenCategoryDoesNotExist()
    {
        var dto = new ExerciseCreateUpdateDto
        {
            Name = "Deadlift",
            Description = "Vježba za cijelo tijelo",
            YoutubeUrl = "https://youtube.com/deadlift",
            ExerciseCategoryId = 999,
            MuscleGroupId = 1
        };

        var ex = await Task.Run(() => Assert.ThrowsAsync<ArgumentException>(async () => await _service.CreateAsync(dto)));

        Assert.That(ex!.Message, Is.EqualTo("Kategorija vježbe ne postoji."));
    }

    [Test]
    public async Task CreateAsync_ThrowsArgumentException_WhenMuscleGroupDoesNotExist()
    {
        var dto = new ExerciseCreateUpdateDto
        {
            Name = "Deadlift",
            Description = "Vježba za cijelo tijelo",
            YoutubeUrl = "https://youtube.com/deadlift",
            ExerciseCategoryId = 1,
            MuscleGroupId = 999
        };

        var ex = await Task.Run(() => Assert.ThrowsAsync<ArgumentException>(async () => await _service.CreateAsync(dto)));

        Assert.That(ex!.Message, Is.EqualTo("Mišićna skupina ne postoji."));
    }

    [Test]
    public async Task UpdateAsync_ReturnsFalse_WhenExerciseDoesNotExist()
    {
        var dto = new ExerciseCreateUpdateDto
        {
            Name = "Updated",
            Description = "Updated description",
            YoutubeUrl = "https://youtube.com/updated",
            ExerciseCategoryId = 1,
            MuscleGroupId = 1
        };

        var result = await _service.UpdateAsync(999, dto);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task UpdateAsync_UpdatesExercise_WhenDtoIsValid()
    {
        var dto = new ExerciseCreateUpdateDto
        {
            Name = "Bench Press Updated",
            Description = "Nova verzija opisa",
            YoutubeUrl = "https://youtube.com/updated",
            ExerciseCategoryId = 2,
            MuscleGroupId = 2
        };

        var result = await _service.UpdateAsync(1, dto);

        Assert.That(result, Is.True);

        var updatedExercise = await _context.Exercises.FirstAsync(x => x.Id == 1);
        Assert.That(updatedExercise.Name, Is.EqualTo("Bench Press Updated"));
        Assert.That(updatedExercise.Description, Is.EqualTo("Nova verzija opisa"));
        Assert.That(updatedExercise.YoutubeUrl, Is.EqualTo("https://youtube.com/updated"));
        Assert.That(updatedExercise.ExerciseCategoryId, Is.EqualTo(2));
        Assert.That(updatedExercise.MuscleGroupId, Is.EqualTo(2));
    }

    [Test]
    public async Task DeleteAsync_ReturnsFalse_WhenExerciseDoesNotExist()
    {
        var result = await _service.DeleteAsync(999);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task DeleteAsync_RemovesExercise_WhenExists()
    {
        var result = await _service.DeleteAsync(1);

        Assert.That(result, Is.True);

        var deletedExercise = await _context.Exercises.FirstOrDefaultAsync(x => x.Id == 1);
        Assert.That(deletedExercise, Is.Null);
    }
}