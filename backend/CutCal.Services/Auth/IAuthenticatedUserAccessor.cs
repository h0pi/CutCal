namespace CutCal.Services.Auth;

public interface IAuthenticatedUserAccessor
{
    int UserId { get; }
    string? Role { get; }
    bool IsInRole(string role);
}
