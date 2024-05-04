using SimpleLoginUI.ViewModels.Dashboard;

namespace SimpleLoginUI.Views.Dashboard;

public partial class StudentDashboardPage : ContentPage
{
    StudentDashboardPageViewModel _viewmodel;

    public StudentDashboardPage(StudentDashboardPageViewModel adminDashboardPageviewModel)
    {
        InitializeComponent();
        _viewmodel = adminDashboardPageviewModel;
        this.BindingContext = _viewmodel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewmodel.RunTasksSynchronously();
    }
}