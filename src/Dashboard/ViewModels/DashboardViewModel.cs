using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using RealTimeInfoDashboard.Services;
using SkiaSharp;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace RealTimeInfoDashboard.ViewModels;

/// <summary>
/// Drives the real-time factory telemetry dashboard.
/// 
/// <para><b>Gauge</b> – a 180° pie-chart arc with four coloured sections
/// (DarkRed 60-65, Orange 65-70, Green 70-85, Orange 85-90) and a
/// WPF <c>Line</c> needle whose <see cref="NeedleAngle"/> is data-bound
/// via <see cref="RotateTransform"/>.</para>
///
/// <para><b>Cartesian chart</b> – five series (Pulse step-line, Efficiency,
/// Red, Green, Blue) sharing a 30-second sliding window on the X axis.</para>
/// </summary>
public partial class DashboardViewModel : ObservableObject
{
    // ── Dependencies ──────────────────────────────────────────────────
    private readonly ITelemetryService _telemetryService;
    private CancellationTokenSource? _cts;

    /// <summary>Epoch used to convert CSV timestamps into chart-friendly seconds.</summary>
    private static readonly DateTime TimeZero =
        DateTime.Parse("2018-01-01T08:00:00Z").ToUniversalTime();

    /// <summary>Width of the sliding X-axis window in seconds.</summary>
    private const double WindowSeconds = 30;

    /// <summary>Maximum number of data points kept per series.</summary>
    private const int MaxPoints = 300;

    // ── Observable collections (one per series) ──────────────────────
    public ObservableCollection<ObservablePoint> EfficiencyValues { get; } = [];
    public ObservableCollection<ObservablePoint> PulseValues { get; } = [];
    public ObservableCollection<ObservablePoint> RedValues { get; } = [];
    public ObservableCollection<ObservablePoint> GreenValues { get; } = [];
    public ObservableCollection<ObservablePoint> BlueValues { get; } = [];

    // ── Gauge state ──────────────────────────────────────────────────
    /// <summary>Current engine efficiency displayed below the gauge arc.</summary>
    [ObservableProperty]
    private double _efficiency = 65;

    /// <summary>
    /// Rotation angle for the WPF needle <see cref="Line"/>.
    /// Maps the 60-90 efficiency range to -90° … +90°.
    /// </summary>
    [ObservableProperty]
    private double _needleAngle = -60;

    // ── Series / Axes exposed to XAML bindings ───────────────────────
    public ISeries[] ChartSeries { get; }
    public Axis[] XAxes { get; }
    public Axis[] YAxes { get; }

    // ═════════════════════════════════════════════════════════════════
    //  Construction
    // ═════════════════════════════════════════════════════════════════
    public DashboardViewModel(ITelemetryService telemetryService)
    {
        _telemetryService = telemetryService;

        // ── Cartesian series ────────────────────────────────────────
        ChartSeries =
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
            },
            new LineSeries<ObservablePoint>
            {
                Values = RedValues,
                Name = "Red",
                Stroke = new SolidColorPaint(new SKColor(255, 0, 0)) { StrokeThickness = 1 },
                Fill = null,
                GeometrySize = 0,
                LineSmoothness = 1
            },
            new LineSeries<ObservablePoint>
            {
                Values = GreenValues,
                Name = "Green",
                Stroke = new SolidColorPaint(new SKColor(0, 128, 0)) { StrokeThickness = 2 },
                Fill = null,
                GeometrySize = 0,
                LineSmoothness = 1
            },
            new LineSeries<ObservablePoint>
            {
                Values = BlueValues,
                Name = "Blue",
                Stroke = new SolidColorPaint(new SKColor(0, 0, 255)) { StrokeThickness = 3 },
                Fill = null,
                GeometrySize = 0,
                LineSmoothness = 1
            }
        ];

        // ── Axes ────────────────────────────────────────────────────
        XAxes =
        [
            new Axis
            {
                Labeler = v => v.ToString("0"),
                UnitWidth = 1,
                MinStep = 5,
                MinLimit = 0,
                MaxLimit = WindowSeconds
            }
        ];

        YAxes =
        [
            new Axis { MinLimit = 0, MaxLimit = 270 }
        ];
    }

    // ═════════════════════════════════════════════════════════════════
    //  Commands
    // ═════════════════════════════════════════════════════════════════

    /// <summary>
    /// Synchronous toggle so the button remains responsive while data
    /// streams via fire-and-forget.  Clicking again cancels the token.
    /// </summary>
    [RelayCommand]
    private void ToggleDataReading()
    {
        if (_cts is not null)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }
        else
        {
            _cts = new CancellationTokenSource();
            _ = ReadDataAsync(_cts.Token);
        }
    }

    // ═════════════════════════════════════════════════════════════════
    //  Private helpers
    // ═════════════════════════════════════════════════════════════════

    private async Task ReadDataAsync(CancellationToken token)
    {
        try
        {
            await foreach (var ft in _telemetryService.StreamTelemetryAsync(token))
            {
                // ── Gauge ───────────────────────────────────────
                Efficiency = Math.Round(ft.Efficiency, 1);
                UpdateNeedle(ft.Efficiency);

                // ── Chart ───────────────────────────────────────
                double seconds = (ft.TimeStamp - TimeZero).TotalSeconds;

                EfficiencyValues.Add(new ObservablePoint(seconds, ft.Efficiency));
                PulseValues.Add(new ObservablePoint(seconds, ft.Pulse));
                RedValues.Add(new ObservablePoint(seconds, ft.Red));
                GreenValues.Add(new ObservablePoint(seconds, ft.Green));
                BlueValues.Add(new ObservablePoint(seconds, ft.Blue));

                AdjustAxis(seconds);
                TrimOldPoints();
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when the user clicks Stop.
        }
    }

    /// <summary>Keep only the most recent <see cref="MaxPoints"/> per series.</summary>
    private void TrimOldPoints()
    {
        if (EfficiencyValues.Count <= MaxPoints) return;

        EfficiencyValues.RemoveAt(0);
        PulseValues.RemoveAt(0);
        RedValues.RemoveAt(0);
        GreenValues.RemoveAt(0);
        BlueValues.RemoveAt(0);
    }

    /// <summary>Slide the X-axis window to keep the latest data visible.</summary>
    private void AdjustAxis(double seconds)
    {
        XAxes[0].MinLimit = seconds < WindowSeconds ? 0 : seconds - WindowSeconds;
        XAxes[0].MaxLimit = seconds < WindowSeconds ? WindowSeconds : seconds;
    }

    /// <summary>
    /// Map efficiency (60-90) to a rotation angle (-90° … +90°) for
    /// the WPF needle <c>Line</c>.
    /// </summary>
    private void UpdateNeedle(double efficiency)
    {
        var clamped = Math.Clamp(efficiency, 60, 90);
        NeedleAngle = (clamped - 60) / 30.0 * 180.0 - 90.0;
    }

}
