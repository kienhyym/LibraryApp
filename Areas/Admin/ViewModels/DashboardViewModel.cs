namespace LibraryApp.Areas.Admin.ViewModels;

public class DashboardViewModel
{
    public int TotalBooks { get; set; }

    public int TotalAuthors { get; set; }

    public int TotalCategories { get; set; }

    public int TotalResidents { get; set; }

    public int TotalBorrowRecords { get; set; }

    public int BorrowingBooks { get; set; }

    public int OverdueBooks { get; set; }
}