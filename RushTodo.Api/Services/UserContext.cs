namespace RushTodo.Api.Services;

public interface IUserContext
{
    int AppUserId { get; }
    TimeZoneInfo TimeZone { get; }
}


public class UserContext : IUserContext
{
    public int AppUserId => 1;
    public TimeZoneInfo TimeZone { get; } = TimeZoneInfo.FindSystemTimeZoneById("Pacific/Auckland");
}
