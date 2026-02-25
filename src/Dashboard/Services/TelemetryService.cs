using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RealTimeInfoDashboard.Models;

namespace RealTimeInfoDashboard.Services;

public class TelemetryService : ITelemetryService
{
    private readonly string _dataFile;

    public TelemetryService(string dataFile = @"data\dashBoardData.csv")
    {
        _dataFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dataFile);
    }

    public async IAsyncEnumerable<FactoryTelemetry> StreamTelemetryAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // For simulation purposes, we read the static file but yield items slowly
        // to emulate a real-time data ingestion stream.
        var dataPoints = FactoryTelemetry.Load(_dataFile);
        
        foreach (var ft in dataPoints)
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;

            yield return ft;
            
            // Wait 50ms between points to match original dashboard speed
            await Task.Delay(50, cancellationToken);
        }
    }
}
