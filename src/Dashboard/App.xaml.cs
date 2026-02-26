using System;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using RealTimeInfoDashboard.Services;
using RealTimeInfoDashboard.ViewModels;

namespace RealTimeInfoDashboard;

/// <summary>
/// Application root.  Configures the DI container and installs global
/// exception handlers that write diagnostic logs next to the executable.
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

        services.AddTransient<DashboardViewModel>();
        services.AddSingleton<ITelemetryService, TelemetryService>();

        return services.BuildServiceProvider();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        DispatcherUnhandledException += (_, args) =>
        {
            WriteCrashLog("crash.log", args.Exception);
            args.Handled = true;
        };

        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
        {
            if (args.ExceptionObject is Exception ex)
                WriteCrashLog("crash_domain.log", ex);
        };

        TaskScheduler.UnobservedTaskException += (_, args) =>
        {
            WriteCrashLog("crash_task.log", args.Exception);
        };
    }

    private static void WriteCrashLog(string filename, Exception ex)
    {
        try
        {
            var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);
            System.IO.File.WriteAllText(path, $"[{DateTime.UtcNow:O}]\n{ex}");
        }
        catch
        {
            // Last-resort: if we can't write the log, don't crash the crash handler.
        }
    }
}
