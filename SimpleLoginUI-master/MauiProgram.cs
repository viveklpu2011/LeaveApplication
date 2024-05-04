using CommunityToolkit.Maui;
using SimpleLoginUI.Handlers;
using SimpleLoginUI.ViewModels;
using SimpleLoginUI.ViewModels.Dashboard;
using SimpleLoginUI.ViewModels.Startup;
using SimpleLoginUI.Views.Dashboard;
using SimpleLoginUI.Views.Startup;

namespace SimpleLoginUI;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

        //Views
        builder.Services.AddSingleton<LoginPage>();
        builder.Services.AddSingleton<DashboardPage>();
        builder.Services.AddSingleton<LoadingPage>();
		builder.Services.AddSingleton<StudentDashboardPage>();
        builder.Services.AddTransient<AdminDashboardPage>();
        builder.Services.AddTransient<TeacherDashboardPage>();

        //View Models
        builder.Services.AddSingleton<LoginPageViewModel>();
        builder.Services.AddSingleton<DashboardPageViewModel>();
        builder.Services.AddSingleton<LoadingPageViewModel>();
        builder.Services.AddSingleton<SQLiteDBContext>();
        builder.Services.AddSingleton<StudentDashboardPageViewModel>();
		builder.Services.AddTransient<AdminDashboardPageviewModel>();
        builder.Services.AddTransient<TeacherDashboardPageViewModel>();

        return builder.Build();
	}
}
