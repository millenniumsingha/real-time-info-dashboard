using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

using Microsoft.Extensions.DependencyInjection;
using RealTimeInfoDashboard.ViewModels;

namespace RealTimeInfoDashboard;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        // Resolve ViewModel from DI Container
        DataContext = App.Current.Services.GetRequiredService<DashboardViewModel>();
    }
}
