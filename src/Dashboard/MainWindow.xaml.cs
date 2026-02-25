using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using RealTimeInfoDashboard.ViewModels;

namespace RealTimeInfoDashboard;

/// <summary>
/// Root application window. Resolves <see cref="DashboardViewModel"/> from the
/// DI container and assigns it as the <see cref="FrameworkElement.DataContext"/>.
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = App.Current.Services.GetRequiredService<DashboardViewModel>();
    }
}
