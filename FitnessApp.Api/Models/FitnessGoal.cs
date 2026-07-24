namespace FitnessApp.Api.Models;

public class FitnessGoal
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<UserProfile> UserProfiles { get; set; } = new List<UserProfile>();
}