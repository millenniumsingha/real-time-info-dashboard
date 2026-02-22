using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using RealTimeInfoDashboard.Services;

namespace RealTimeInfoDashboard.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly ITelemetryService _telemetryService;
    private CancellationTokenSource? _cancellationTokenSource;

    [ObservableProperty]
    private double _engineEfficiency = 65;

    public DashboardViewModel(ITelemetryService telemetryService)
    {
        _telemetryService = telemetryService;
    }

    [RelayCommand]
    private async Task ToggleDataReadingAsync()
    {
        if (_cancellationTokenSource != null)
        {
            // Stop reading
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }
        else
        {
            // Start reading
            _cancellationTokenSource = new CancellationTokenSource();
            await ReadDataAsync(_cancellationTokenSource.Token);
        }
    }

    private async Task ReadDataAsync(CancellationToken token)
    {
        await foreach (var ft in _telemetryService.StreamTelemetryAsync(token))
        {
            // Emulate the original logic from ConstantChangesChart.xaml.cs
            EngineEfficiency = ft.Efficiency;
            
            // To be implemented in next tasks:
            // 1. ChartValues.Add(ft)
            // 2. AdjustAxis(ft.TimeStamp.Ticks)
            // 3. Sliding window removal if Count > 30
        }
    }
}
