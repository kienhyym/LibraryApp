namespace LibraryApp.Configurations;

public class EmailSettings
{
    public string SenderName { get; set; } = string.Empty;

    public string SenderEmail { get; set; } = string.Empty;

    public string AppPassword { get; set; } = string.Empty;

    public string SmtpServer { get; set; } = string.Empty;

    public int Port { get; set; }
}