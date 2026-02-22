using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Extensions;
using SkiaSharp;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using RealTimeInfoDashboard.Services;

namespace RealTimeInfoDashboard.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly ITelemetryService _telemetryService;
    private CancellationTokenSource? _cancellationTokenSource;

    private static readonly long TickZero = DateTime.Parse("2018-01-01T08:00:00Z").Ticks;

    public ObservableCollection<ObservablePoint> EfficiencyValues { get; } = new();
    public ObservableCollection<ObservablePoint> PulseValues { get; } = new();
    
    public ObservableCollection<ObservablePoint> RedValues { get; } = new();
    public ObservableCollection<ObservablePoint> GreenValues { get; } = new();
    public ObservableCollection<ObservablePoint> BlueValues { get; } = new();

    public ObservableValue GaugeValue { get; } = new(65);

    public ISeries[] MainSeries { get; set; }
    public ISeries[] RgbSeries { get; set; }
    public ISeries[] GaugeSeries { get; set; }

    public Axis[] XAxes { get; set; }
    public Axis[] YAxes { get; set; }

    public DashboardViewModel(ITelemetryService telemetryService)
    {
        _telemetryService = telemetryService;

        MainSeries =
        [
            new StepLineSeries<ObservablePoint>
            {
                Values = PulseValues,
                Name = "Pulse",
                Stroke = new SolidColorPaint(SKColors.HotPink) { StrokeThickness = 2 },
                Fill = null,
                GeometrySize = 0
            },
            new LineSeries<ObservablePoint>
            {
                Values = EfficiencyValues,
                Name = "Efficiency",
                Stroke = new SolidColorPaint(SKColors.Black) { StrokeThickness = 2 },
                Fill = null,
                GeometrySize = 0,
                LineSmoothness = 0
            }
        ];

        RgbSeries =
        [
            new LineSeries<ObservablePoint>
            {
                Values = RedValues,
                Name = "Red",
                Stroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 1 },
                Fill = null,
                GeometrySize = 0,
                LineSmoothness = 1
            },
            new LineSeries<ObservablePoint>
            {
                Values = GreenValues,
                Name = "Green",
                Stroke = new SolidColorPaint(SKColors.Green) { StrokeThickness = 2 },
                Fill = null,
                GeometrySize = 0,
                LineSmoothness = 1
            },
            new LineSeries<ObservablePoint>
            {
                Values = BlueValues,
                Name = "Blue",
                Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 3 },
                Fill = null,
                GeometrySize = 0,
                LineSmoothness = 1
            }
        ];

        GaugeSeries =
        [
            new PieSeries<ObservableValue>
            {
                Values = new[] { GaugeValue },
                Name = "Efficiency",
                InnerRadius = 50,
                MaxRadialColumnWidth = 50,
                Fill = new SolidColorPaint(SKColors.Green)
            }
        ];

        XAxes = [
            new Axis
            {
                Labeler = value => TimeSpan.FromTicks((long)value - TickZero).TotalSeconds.ToString("0.0"),
                UnitWidth = TimeSpan.FromSeconds(1).Ticks,
                MinStep = TimeSpan.FromSeconds(5).Ticks,
                MinLimit = TickZero,
                MaxLimit = TickZero + TimeSpan.FromSeconds(30).Ticks
            }
        ];

        YAxes = [
            new Axis
            {
                MinLimit = 0,
                MaxLimit = 270
            }
        ];
    }

    [RelayCommand]
    private async Task ToggleDataReadingAsync()
    {
        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }
        else
        {
            _cancellationTokenSource = new CancellationTokenSource();
            await ReadDataAsync(_cancellationTokenSource.Token);
        }
    }

    private async Task ReadDataAsync(CancellationToken token)
    {
        await foreach (var ft in _telemetryService.StreamTelemetryAsync(token))
        {
            GaugeValue.Value = ft.Efficiency;

            EfficiencyValues.Add(new ObservablePoint(ft.TimeStamp.Ticks, ft.Efficiency));
            PulseValues.Add(new ObservablePoint(ft.TimeStamp.Ticks, ft.Pulse));
            RedValues.Add(new ObservablePoint(ft.TimeStamp.Ticks, ft.Red));
            GreenValues.Add(new ObservablePoint(ft.TimeStamp.Ticks, ft.Green));
            BlueValues.Add(new ObservablePoint(ft.TimeStamp.Ticks, ft.Blue));

            AdjustAxis(ft.TimeStamp.Ticks);

            if (EfficiencyValues.Count > 30)
            {
                EfficiencyValues.RemoveAt(0);
                PulseValues.RemoveAt(0);
                RedValues.RemoveAt(0);
                GreenValues.RemoveAt(0);
                BlueValues.RemoveAt(0);
            }
        }
    }

    private void AdjustAxis(long ticks)
    {
        var width = TimeSpan.FromSeconds(30).Ticks;

        XAxes[0].MinLimit = (ticks - TickZero < width) ? TickZero : ticks - width;
        XAxes[0].MaxLimit = (ticks - TickZero < width) ? TickZero + width : ticks;
    }
}
