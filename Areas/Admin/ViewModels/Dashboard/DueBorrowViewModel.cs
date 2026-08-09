namespace LibraryApp.Areas.Admin.ViewModels.Dashboard;
public class DueBorrowViewModel
{
    public int BorrowRecordId { get; set; }

    public string ResidentName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public DateTime DueDate { get; set; }

    public int RemainingDays { get; set; }

    public int TotalBooks { get; set; }

    public string StatusText =>
        RemainingDays switch
        {
            0 => "Hôm nay",

            1 => "Còn 1 ngày",

            _ => $"Còn {RemainingDays} ngày"
        };

    public string BadgeClass =>
        RemainingDays switch
        {
            0 => "bg-danger",

            1 => "bg-warning text-dark",

            _ => "bg-success"
        };
}