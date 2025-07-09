// BlazorLearning.Shared/Models/UserState.cs
namespace BlazorLearning.Shared.Models;

public class UserState
{
    public bool IsAuthenticated { get; set; } = false;
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public DateTime LoginTime { get; set; } = DateTime.Now;
}