using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RealTimeInfoDashboard.Models;

namespace RealTimeInfoDashboard.Services;

public interface ITelemetryService
{
    IAsyncEnumerable<FactoryTelemetry> StreamTelemetryAsync(CancellationToken cancellationToken);
}
