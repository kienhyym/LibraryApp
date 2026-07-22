namespace LibraryApp.Results;

public class LoginResult
{
    public bool Success { get; set; }

    public string? ErrorMessage { get; set; }

    public int? AccountId { get; set; }

    public string? Email { get; set; }

    public string? Role { get; set; }
}