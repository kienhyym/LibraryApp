namespace LibraryApp.Areas.Admin.ViewModels.Dashboard;
public class BorrowChartViewModel
{
    public int Year { get; set; }

    public List<string> Labels { get; set; } = [];

    public List<int> Values { get; set; } = [];
}