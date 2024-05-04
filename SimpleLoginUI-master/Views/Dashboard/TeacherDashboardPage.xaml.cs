using SimpleLoginUI.Models;
using SimpleLoginUI.ViewModels.Dashboard;
using static Android.Content.ClipData;

namespace SimpleLoginUI.Views.Dashboard;

public partial class TeacherDashboardPage : ContentPage
{
    TeacherDashboardPageViewModel _teacherDashboardPageViewModel;

    public TeacherDashboardPage(TeacherDashboardPageViewModel teacherDashboardPageViewModel)
    {
        InitializeComponent();
        _teacherDashboardPageViewModel = teacherDashboardPageViewModel;
        this.BindingContext = _teacherDashboardPageViewModel;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await _teacherDashboardPageViewModel.LoadLeaves();
    }

}