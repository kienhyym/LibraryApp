namespace LibraryApp.Areas.Admin.ViewModels.Dashboard;
public class DashboardViewModel
{
    public DashboardStatisticViewModel Statistics { get; set; }
        = new();

    public BorrowChartViewModel BorrowChart { get; set; }
        = new();

    public List<TopItemViewModel> TopBooks { get; set; }
        = [];

    public List<TopItemViewModel> TopCategories { get; set; }
        = [];

    public List<TopItemViewModel> TopAuthors { get; set; }
        = [];

    public List<DueBorrowViewModel> DueBorrows { get; set; }
        = [];
}