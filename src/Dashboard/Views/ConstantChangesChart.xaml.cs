using RealTimeInfoDashboard.Models;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RealTimeInfoDashboard.Views;

/// <summary>
/// Interaction logic for ConstantChangesChart.xaml
/// </summary>
public partial class ConstantChangesChart : UserControl, INotifyPropertyChanged
{
        public ConstantChangesChart()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Logic disabled for Phase 2 .NET 10 build verification
        }

        public double EngineEfficiency { get; set; } = 65;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
