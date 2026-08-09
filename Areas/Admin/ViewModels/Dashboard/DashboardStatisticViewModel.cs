namespace LibraryApp.Areas.Admin.ViewModels.Dashboard;
public class DashboardStatisticViewModel
{
    //70%

    public int TotalBooks { get; set; }

    public int TotalAuthors { get; set; }

    public int TotalCategories { get; set; }

    public int TotalResidents { get; set; }

    public int TotalBorrowRecords { get; set; }

    //30%

    public int BorrowingRecords { get; set; }

    public int OverdueRecords { get; set; }
}