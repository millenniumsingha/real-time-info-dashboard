using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Extensions.DependencyInjection;
using RealTimeInfoDashboard.Services;
using RealTimeInfoDashboard.ViewModels;

namespace RealTimeInfoDashboard;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public IServiceProvider Services { get; }

    public new static App Current => (App)Application.Current;

    public App()
    {
        Services = ConfigureServices();
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // ViewModels
        services.AddTransient<DashboardViewModel>();

        // Services
        services.AddSingleton<ITelemetryService, TelemetryService>();

        return services.BuildServiceProvider();
    }
}
