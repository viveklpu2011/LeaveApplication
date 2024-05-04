using AndroidX.Lifecycle;
using SimpleLoginUI.ViewModels.Dashboard;

namespace SimpleLoginUI.Views.Dashboard;

public partial class AdminDashboardPage : ContentPage
{
    AdminDashboardPageviewModel _adminDashboardPageviewModel;

    public AdminDashboardPage(AdminDashboardPageviewModel adminDashboardPageviewModel)
	{
		InitializeComponent();
        _adminDashboardPageviewModel = adminDashboardPageviewModel;
        this.BindingContext = _adminDashboardPageviewModel;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await _adminDashboardPageviewModel.LoadLeaves();
    }
}