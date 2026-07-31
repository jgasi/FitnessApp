using FitnessApp.Api.Data;
using FitnessApp.Api.DTOs;
using FitnessApp.Api.Models;
using FitnessApp.Api.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace FitnessApp.Api.Tests.Services;

[TestFixture]
public class CalorieEntryServiceTests
{
    private const string CurrentUserId = "user-1";
    private const string OtherUserId = "user-2";

    private ApplicationDbContext _context = null!;
    private IUnitOfWork _unitOfWork = null!;
    private CalorieEntryService _service = null!;

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
        _service = new CalorieEntryService(_unitOfWork);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public async Task GetAllAsync_ReturnsOnlyCurrentUsersEntries_WhenNotAdmin()
    {
        var result = (await _service.GetAllAsync(CurrentUserId, isAdmin: false)).ToList();

        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.All(x => x.UserId == CurrentUserId), Is.True);
        Assert.That(result[0].EntryDate, Is.EqualTo(new DateTime(2026, 1, 2)));
        Assert.That(result[1].EntryDate, Is.EqualTo(new DateTime(2026, 1, 1)));
    }

    [Test]
    public async Task GetAllAsync_ReturnsAllEntries_WhenAdmin()
    {
        var result = (await _service.GetAllAsync(CurrentUserId, isAdmin: true)).ToList();

        Assert.That(result.Count, Is.EqualTo(3));
        Assert.That(result.Any(x => x.UserId == OtherUserId), Is.True);
    }

    [Test]
    public async Task GetByIdAsync_ReturnsNull_WhenEntryDoesNotExist()
    {
        var result = await _service.GetByIdAsync(999, CurrentUserId, isAdmin: false);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetByIdAsync_ReturnsNull_WhenEntryBelongsToAnotherUser_AndNotAdmin()
    {
        var result = await _service.GetByIdAsync(3, CurrentUserId, isAdmin: false);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetByIdAsync_ReturnsEntry_WhenAdmin()
    {
        var result = await _service.GetByIdAsync(3, CurrentUserId, isAdmin: true);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.UserId, Is.EqualTo(OtherUserId));
        Assert.That(result.Calories, Is.EqualTo(3000));
    }

    [Test]
    public async Task CreateAsync_ThrowsArgumentException_WhenDateIsInFuture()
    {
        var dto = new CalorieEntryCreateUpdateDto
        {
            EntryDate = DateTime.UtcNow.Date.AddDays(1),
            Calories = 2400,
            Notes = "Future date"
        };

        var ex = await Task.Run(() => Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.CreateAsync(CurrentUserId, dto)));

        Assert.That(ex!.Message, Is.EqualTo("Datum unosa kalorija ne može biti u budućnosti."));
    }

    [Test]
    public async Task CreateAsync_ThrowsArgumentException_WhenDuplicateEntryForSameDateExists()
    {
        var dto = new CalorieEntryCreateUpdateDto
        {
            EntryDate = new DateTime(2026, 1, 1),
            Calories = 2400,
            Notes = "Duplicate"
        };

        var ex = await Task.Run(() => Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.CreateAsync(CurrentUserId, dto)));

        Assert.That(ex!.Message, Is.EqualTo("Unos kalorija za taj datum već postoji."));
    }

    [Test]
    public async Task CreateAsync_AddsEntry_WhenValid()
    {
        var dto = new CalorieEntryCreateUpdateDto
        {
            EntryDate = new DateTime(2026, 1, 3),
            Calories = 2500,
            Notes = "Novi unos"
        };

        var result = await _service.CreateAsync(CurrentUserId, dto);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.UserId, Is.EqualTo(CurrentUserId));
        Assert.That(result.EntryDate, Is.EqualTo(new DateTime(2026, 1, 3)));
        Assert.That(result.Calories, Is.EqualTo(2500));

        var dbEntry = await _context.CalorieEntries.FirstOrDefaultAsync(x =>
            x.UserId == CurrentUserId && x.EntryDate == new DateTime(2026, 1, 3));

        Assert.That(dbEntry, Is.Not.Null);
    }

    [Test]
    public async Task UpdateAsync_ThrowsArgumentException_WhenDateIsInFuture()
    {
        var dto = new CalorieEntryCreateUpdateDto
        {
            EntryDate = DateTime.UtcNow.Date.AddDays(1),
            Calories = 2600,
            Notes = "Future update"
        };

        var ex = await Task.Run(() => Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.UpdateAsync(1, CurrentUserId, isAdmin: false, dto)));

        Assert.That(ex!.Message, Is.EqualTo("Datum unosa kalorija ne može biti u budućnosti."));
    }

    [Test]
    public async Task UpdateAsync_ReturnsFalse_WhenEntryDoesNotExist()
    {
        var dto = new CalorieEntryCreateUpdateDto
        {
            EntryDate = new DateTime(2026, 1, 3),
            Calories = 2600,
            Notes = "Missing"
        };

        var result = await _service.UpdateAsync(999, CurrentUserId, isAdmin: false, dto);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task UpdateAsync_ReturnsFalse_WhenEntryBelongsToAnotherUser_AndNotAdmin()
    {
        var dto = new CalorieEntryCreateUpdateDto
        {
            EntryDate = new DateTime(2026, 1, 3),
            Calories = 2600,
            Notes = "Unauthorized"
        };

        var result = await _service.UpdateAsync(3, CurrentUserId, isAdmin: false, dto);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task UpdateAsync_ThrowsArgumentException_WhenDuplicateDateExists()
    {
        var dto = new CalorieEntryCreateUpdateDto
        {
            EntryDate = new DateTime(2026, 1, 2),
            Calories = 2600,
            Notes = "Duplicate date"
        };

        var ex = await Task.Run(() => Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.UpdateAsync(1, CurrentUserId, false, dto)));

        Assert.That(ex!.Message, Is.EqualTo("Unos kalorija za taj datum već postoji."));
    }

    [Test]
    public async Task UpdateAsync_UpdatesEntry_WhenValid()
    {
        var dto = new CalorieEntryCreateUpdateDto
        {
            EntryDate = new DateTime(2026, 1, 3),
            Calories = 2600,
            Notes = "Ažurirano"
        };

        var result = await _service.UpdateAsync(1, CurrentUserId, isAdmin: false, dto);

        Assert.That(result, Is.True);

        var updated = await _context.CalorieEntries.FirstAsync(x => x.Id == 1);
        Assert.That(updated.EntryDate, Is.EqualTo(new DateTime(2026, 1, 3)));
        Assert.That(updated.Calories, Is.EqualTo(2600));
        Assert.That(updated.Notes, Is.EqualTo("Ažurirano"));
    }

    [Test]
    public async Task DeleteAsync_ReturnsFalse_WhenEntryDoesNotExist()
    {
        var result = await _service.DeleteAsync(999, CurrentUserId, isAdmin: false);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task DeleteAsync_ReturnsFalse_WhenEntryBelongsToAnotherUser_AndNotAdmin()
    {
        var result = await _service.DeleteAsync(3, CurrentUserId, isAdmin: false);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task DeleteAsync_RemovesEntry_WhenValid()
    {
        var result = await _service.DeleteAsync(1, CurrentUserId, isAdmin: false);

        Assert.That(result, Is.True);

        var deleted = await _context.CalorieEntries.FirstOrDefaultAsync(x => x.Id == 1);
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

        _context.CalorieEntries.AddRange(
            new CalorieEntry
            {
                Id = 1,
                UserId = CurrentUserId,
                EntryDate = new DateTime(2026, 1, 1),
                Calories = 2000,
                Notes = "Prvi unos"
            },
            new CalorieEntry
            {
                Id = 2,
                UserId = CurrentUserId,
                EntryDate = new DateTime(2026, 1, 2),
                Calories = 2200,
                Notes = "Drugi unos"
            },
            new CalorieEntry
            {
                Id = 3,
                UserId = OtherUserId,
                EntryDate = new DateTime(2026, 1, 1),
                Calories = 3000,
                Notes = "Other user"
            });
    }
}