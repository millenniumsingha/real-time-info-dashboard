using System.Windows.Controls;

namespace RealTimeInfoDashboard.Views;

/// <summary>
/// Real-time telemetry chart with an angular gauge and 5-series Cartesian overlay.
/// All logic is in <see cref="ViewModels.DashboardViewModel"/>; this code-behind
/// only initialises the XAML component tree.
/// </summary>
public partial class ConstantChangesChart : UserControl
{
    public ConstantChangesChart()
    {
        InitializeComponent();
    }
}
