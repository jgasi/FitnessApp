using FitnessApp.Api.Data;
using FitnessApp.Api.Models;
using FitnessApp.Api.Services.Implementations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace FitnessApp.Api.Tests.Services;

[TestFixture]
public class ProfileServiceTests
{
    private const string CurrentUserId = "user-1";
    private const string OtherUserId = "user-2";

    private ApplicationDbContext _context = null!;
    private IUnitOfWork _unitOfWork = null!;
    private Mock<UserManager<ApplicationUser>> _userManagerMock = null!;
    private ProfileService _service = null!;

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
        _userManagerMock = CreateUserManagerMock(_context);
        _service = new ProfileService(_unitOfWork, _userManagerMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public async Task GetMyProfileAsync_ReturnsProfile_WhenUserAndProfileExist()
    {
        var result = await _service.GetMyProfileAsync(CurrentUserId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.UserId, Is.EqualTo(CurrentUserId));
        Assert.That(result.FirstName, Is.EqualTo("Marko"));
        Assert.That(result.LastName, Is.EqualTo("Markovic"));
        Assert.That(result.Email, Is.EqualTo("marko@test.com"));
        Assert.That(result.FitnessGoalId, Is.EqualTo(1));
        Assert.That(result.FitnessGoalName, Is.EqualTo("Mršavljenje"));
        Assert.That(result.HeightCm, Is.EqualTo(180m));
        Assert.That(result.CurrentWeightKg, Is.EqualTo(80m));
    }

    [Test]
    public async Task GetMyProfileAsync_ReturnsNull_WhenUserDoesNotExist()
    {
        _userManagerMock
            .Setup(x => x.FindByIdAsync("missing-user"))
            .ReturnsAsync((ApplicationUser?)null);

        var result = await _service.GetMyProfileAsync("missing-user");

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetMyProfileAsync_ReturnsNull_WhenProfileDoesNotExist()
    {
        var result = await _service.GetMyProfileAsync(OtherUserId);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task UpdateMyProfileAsync_ReturnsFalse_WhenProfileDoesNotExist()
    {
        var result = await _service.UpdateMyProfileAsync("missing-user", new FitnessApp.Api.DTOs.UserProfileUpdateDto
        {
            FitnessGoalId = null,
            DateOfBirth = null,
            Gender = null,
            HeightCm = null,
            CurrentWeightKg = null
        });

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task UpdateMyProfileAsync_ThrowsArgumentException_WhenFitnessGoalDoesNotExist()
    {
        var dto = new FitnessApp.Api.DTOs.UserProfileUpdateDto
        {
            FitnessGoalId = 999,
            DateOfBirth = new DateTime(1995, 1, 1),
            Gender = "Muško",
            HeightCm = 185m,
            CurrentWeightKg = 82m
        };

        var ex = await Task.Run(() => Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.UpdateMyProfileAsync(CurrentUserId, dto)));

        Assert.That(ex!.Message, Is.EqualTo("Fitness cilj ne postoji."));
    }

    [Test]
    public async Task UpdateMyProfileAsync_UpdatesProfile_WhenDtoIsValid()
    {
        var dto = new FitnessApp.Api.DTOs.UserProfileUpdateDto
        {
            FitnessGoalId = 2,
            DateOfBirth = new DateTime(1994, 5, 20),
            Gender = "Muško",
            HeightCm = 182m,
            CurrentWeightKg = 78.5m
        };

        var result = await _service.UpdateMyProfileAsync(CurrentUserId, dto);

        Assert.That(result, Is.True);

        var updatedProfile = await _context.UserProfiles.FirstAsync(x => x.UserId == CurrentUserId);
        Assert.That(updatedProfile.FitnessGoalId, Is.EqualTo(2));
        Assert.That(updatedProfile.DateOfBirth, Is.EqualTo(new DateTime(1994, 5, 20)));
        Assert.That(updatedProfile.Gender, Is.EqualTo("Muško"));
        Assert.That(updatedProfile.HeightCm, Is.EqualTo(182m));
        Assert.That(updatedProfile.CurrentWeightKg, Is.EqualTo(78.5m));
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
            new FitnessGoal
            {
                Id = 1,
                Name = "Mršavljenje"
            },
            new FitnessGoal
            {
                Id = 2,
                Name = "Dobivanje mase"
            });

        _context.UserProfiles.Add(
            new UserProfile
            {
                Id = 1,
                UserId = CurrentUserId,
                FitnessGoalId = 1,
                DateOfBirth = new DateTime(1995, 1, 1),
                Gender = "Muško",
                HeightCm = 180m,
                CurrentWeightKg = 80m
            });
    }

    private static Mock<UserManager<ApplicationUser>> CreateUserManagerMock(ApplicationDbContext context)
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        var options = Options.Create(new IdentityOptions());
        var passwordHasher = new PasswordHasher<ApplicationUser>();
        var userValidators = Array.Empty<IUserValidator<ApplicationUser>>();
        var passwordValidators = Array.Empty<IPasswordValidator<ApplicationUser>>();
        var normalizer = new UpperInvariantLookupNormalizer();
        var describer = new IdentityErrorDescriber();
        var services = Mock.Of<IServiceProvider>();
        var logger = Mock.Of<ILogger<UserManager<ApplicationUser>>>();

        var mock = new Mock<UserManager<ApplicationUser>>(
            store.Object,
            options,
            passwordHasher,
            userValidators,
            passwordValidators,
            normalizer,
            describer,
            services,
            logger);

        mock.Setup(x => x.FindByIdAsync(CurrentUserId))
            .ReturnsAsync(context.Users.First(x => x.Id == CurrentUserId));

        mock.Setup(x => x.FindByIdAsync(OtherUserId))
            .ReturnsAsync(context.Users.First(x => x.Id == OtherUserId));

        return mock;
    }
}