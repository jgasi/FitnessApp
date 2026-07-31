using FitnessApp.Api.Data;
using FitnessApp.Api.DTOs;
using FitnessApp.Api.Models;
using FitnessApp.Api.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace FitnessApp.Api.Tests.Services;

[TestFixture]
public class BodyMeasurementServiceTests
{
    private const string CurrentUserId = "user-1";
    private const string OtherUserId = "user-2";

    private ApplicationDbContext _context = null!;
    private IUnitOfWork _unitOfWork = null!;
    private BodyMeasurementService _service = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);

        SeedData();
        _context.SaveChanges();

        _unitOfWork = new UnitOfWork(_context);
        _service = new BodyMeasurementService(_unitOfWork);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public async Task GetAllAsync_ReturnsOnlyCurrentUsersMeasurements_WhenNotAdmin()
    {
        var result = (await _service.GetAllAsync(CurrentUserId, isAdmin: false)).ToList();

        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.All(x => x.UserId == CurrentUserId), Is.True);
        Assert.That(result[0].MeasurementDate, Is.EqualTo(new DateTime(2026, 2, 1)));
    }

    [Test]
    public async Task GetAllAsync_ReturnsAllMeasurements_WhenAdmin()
    {
        var result = (await _service.GetAllAsync(CurrentUserId, isAdmin: true)).ToList();

        Assert.That(result.Count, Is.EqualTo(3));
        Assert.That(result.Any(x => x.UserId == OtherUserId), Is.True);
    }

    [Test]
    public async Task GetByIdAsync_ReturnsNull_WhenMeasurementDoesNotExist()
    {
        var result = await _service.GetByIdAsync(999, CurrentUserId, isAdmin: false);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetByIdAsync_ReturnsNull_WhenMeasurementBelongsToAnotherUser_AndNotAdmin()
    {
        var result = await _service.GetByIdAsync(3, CurrentUserId, isAdmin: false);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetByIdAsync_ReturnsMeasurement_WhenAdmin()
    {
        var result = await _service.GetByIdAsync(3, CurrentUserId, isAdmin: true);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.UserId, Is.EqualTo(OtherUserId));
        Assert.That(result.WeightKg, Is.EqualTo(90m));
    }

    [Test]
    public async Task CreateAsync_ThrowsArgumentException_WhenDateIsInFuture()
    {
        var dto = new BodyMeasurementCreateUpdateDto
        {
            MeasurementDate = DateTime.UtcNow.Date.AddDays(1),
            WeightKg = 82m,
            BodyFatPercentage = 16m,
            Notes = "Future date"
        };

        var ex = await Task.Run(() => Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.CreateAsync(CurrentUserId, dto)));

        Assert.That(ex!.Message, Is.EqualTo("Datum mjerenja ne može biti u budućnosti."));
    }

    [Test]
    public async Task CreateAsync_ThrowsInvalidOperationException_WhenProfileDoesNotExist()
    {
        var dto = new BodyMeasurementCreateUpdateDto
        {
            MeasurementDate = new DateTime(2026, 3, 1),
            WeightKg = 82m,
            BodyFatPercentage = 16m,
            Notes = "No profile"
        };

        var ex = await Task.Run(() => Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.CreateAsync("missing-user", dto)));

        Assert.That(ex!.Message, Is.EqualTo("Profil nije pronađen."));
    }

    [Test]
    public async Task CreateAsync_AddsMeasurementAndUpdatesCurrentWeight_WhenValid()
    {
        var dto = new BodyMeasurementCreateUpdateDto
        {
            MeasurementDate = new DateTime(2026, 3, 1),
            WeightKg = 77m,
            BodyFatPercentage = 15.5m,
            Notes = "Novi unos"
        };

        var result = await _service.CreateAsync(CurrentUserId, dto);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.UserId, Is.EqualTo(CurrentUserId));
        Assert.That(result.WeightKg, Is.EqualTo(77m));
        Assert.That(result.Bmi, Is.EqualTo(23.77m)); // 77 / 1.8^2 = 23.7654 -> 23.77

        var profile = await _context.UserProfiles.FirstAsync(x => x.UserId == CurrentUserId);
        Assert.That(profile.CurrentWeightKg, Is.EqualTo(77m));

        var dbMeasurement = await _context.BodyMeasurements.FirstOrDefaultAsync(x => x.UserId == CurrentUserId && x.WeightKg == 77m);
        Assert.That(dbMeasurement, Is.Not.Null);
    }

    [Test]
    public async Task UpdateAsync_ThrowsArgumentException_WhenDateIsInFuture()
    {
        var dto = new BodyMeasurementCreateUpdateDto
        {
            MeasurementDate = DateTime.UtcNow.Date.AddDays(1),
            WeightKg = 78m,
            BodyFatPercentage = 15m,
            Notes = "Future update"
        };

        var ex = await Task.Run(() => Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.UpdateAsync(1, CurrentUserId, isAdmin: false, dto)));

        Assert.That(ex!.Message, Is.EqualTo("Datum mjerenja ne može biti u budućnosti."));
    }

    [Test]
    public async Task UpdateAsync_ReturnsFalse_WhenMeasurementDoesNotExist()
    {
        var dto = new BodyMeasurementCreateUpdateDto
        {
            MeasurementDate = new DateTime(2026, 3, 1),
            WeightKg = 78m,
            BodyFatPercentage = 15m,
            Notes = "Missing"
        };

        var result = await _service.UpdateAsync(999, CurrentUserId, isAdmin: false, dto);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task UpdateAsync_ReturnsFalse_WhenMeasurementBelongsToAnotherUser_AndNotAdmin()
    {
        var dto = new BodyMeasurementCreateUpdateDto
        {
            MeasurementDate = new DateTime(2026, 3, 1),
            WeightKg = 88m,
            BodyFatPercentage = 20m,
            Notes = "Unauthorized"
        };

        var result = await _service.UpdateAsync(3, CurrentUserId, isAdmin: false, dto);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task UpdateAsync_ThrowsInvalidOperationException_WhenProfileDoesNotExist()
    {
        var dto = new BodyMeasurementCreateUpdateDto
        {
            MeasurementDate = new DateTime(2026, 3, 1),
            WeightKg = 78m,
            BodyFatPercentage = 15m,
            Notes = "No profile"
        };

        _context.UserProfiles.RemoveRange(_context.UserProfiles.Where(x => x.UserId == CurrentUserId));
        await _context.SaveChangesAsync();

        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.UpdateAsync(1, CurrentUserId, isAdmin: false, dto));

        Assert.That(ex!.Message, Is.EqualTo("Profil nije pronađen."));
    }

    [Test]
    public async Task UpdateAsync_UpdatesMeasurementAndCurrentWeight_WhenValid()
    {
        var dto = new BodyMeasurementCreateUpdateDto
        {
            MeasurementDate = new DateTime(2026, 2, 15),
            WeightKg = 76.5m,
            BodyFatPercentage = 15m,
            Notes = "Ažurirano"
        };

        var result = await _service.UpdateAsync(1, CurrentUserId, isAdmin: false, dto);

        Assert.That(result, Is.True);

        var updated = await _context.BodyMeasurements.FirstAsync(x => x.Id == 1);
        Assert.That(updated.WeightKg, Is.EqualTo(76.5m));
        Assert.That(updated.Bmi, Is.EqualTo(23.61m)); // 76.5 / 1.8^2 = 23.6111 -> 23.61

        var profile = await _context.UserProfiles.FirstAsync(x => x.UserId == CurrentUserId);
        Assert.That(profile.CurrentWeightKg, Is.EqualTo(76.5m));
    }

    [Test]
    public async Task DeleteAsync_ReturnsFalse_WhenMeasurementDoesNotExist()
    {
        var result = await _service.DeleteAsync(999, CurrentUserId, isAdmin: false);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task DeleteAsync_ReturnsFalse_WhenMeasurementBelongsToAnotherUser_AndNotAdmin()
    {
        var result = await _service.DeleteAsync(3, CurrentUserId, isAdmin: false);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task DeleteAsync_RemovesMeasurement_WhenValid()
    {
        var result = await _service.DeleteAsync(1, CurrentUserId, isAdmin: false);

        Assert.That(result, Is.True);

        var deleted = await _context.BodyMeasurements.FirstOrDefaultAsync(x => x.Id == 1);
        Assert.That(deleted, Is.Null);
    }

    private void SeedData()
    {
        _context.Users.AddRange(
            new ApplicationUser
            {
                Id = CurrentUserId,
                UserName = "marko",
                Email = "marko@test.com",
                FirstName = "Marko",
                LastName = "Markovic",
                IsActive = true
            },
            new ApplicationUser
            {
                Id = OtherUserId,
                UserName = "ivan",
                Email = "ivan@test.com",
                FirstName = "Ivan",
                LastName = "Ivic",
                IsActive = true
            });

        _context.FitnessGoals.AddRange(
            new FitnessGoal { Id = 1, Name = "Mršavljenje" },
            new FitnessGoal { Id = 2, Name = "Dobivanje mase" });

        _context.UserProfiles.AddRange(
            new UserProfile
            {
                Id = 1,
                UserId = CurrentUserId,
                FitnessGoalId = 1,
                DateOfBirth = new DateTime(1995, 1, 1),
                Gender = "Muško",
                HeightCm = 180m,
                CurrentWeightKg = 80m
            },
            new UserProfile
            {
                Id = 2,
                UserId = OtherUserId,
                FitnessGoalId = 2,
                DateOfBirth = new DateTime(1993, 5, 10),
                Gender = "Muško",
                HeightCm = 175m,
                CurrentWeightKg = 90m
            });

        _context.BodyMeasurements.AddRange(
            new BodyMeasurement
            {
                Id = 1,
                UserId = CurrentUserId,
                MeasurementDate = new DateTime(2026, 1, 1),
                WeightKg = 80m,
                Bmi = 24.69m,
                BodyFatPercentage = 16m,
                Notes = "Početak"
            },
            new BodyMeasurement
            {
                Id = 2,
                UserId = CurrentUserId,
                MeasurementDate = new DateTime(2026, 2, 1),
                WeightKg = 78m,
                Bmi = 24.07m,
                BodyFatPercentage = 15.5m,
                Notes = "Napredak"
            },
            new BodyMeasurement
            {
                Id = 3,
                UserId = OtherUserId,
                MeasurementDate = new DateTime(2026, 1, 15),
                WeightKg = 90m,
                Bmi = 29.39m,
                BodyFatPercentage = 20m,
                Notes = "Other user"
            });
    }
}