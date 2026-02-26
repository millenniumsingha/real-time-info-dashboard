# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

_No unreleased changes._

## [1.0.1] - 2026-02-26

### Fixed
- **Dependabot NuGet Mismatch**: Pinned all LiveCharts packages to `2.0.0-rc5.1` to resolve a fatal `MissingMethodException` caused by a partial version bump (`LiveChartsCore` at `rc6.1` vs `LiveChartsCore.SkiaSharpView.WPF` at `rc5.4`).

### Added
- **ARM64 Release Target**: Release workflow now builds both `win-x64` and `win-arm64` executables via a GitHub Actions build matrix.

## [1.0.0] - 2026-02-26

### Added
- **.NET 10 Migration**: Upgraded the entire project from legacy .NET Framework 4.7.2 to modern .NET 10 SDK-style projects.
- **MVVM Architecture**: Introduced `CommunityToolkit.Mvvm` and Microsoft Dependency Injection for a clean separation of concerns.
- **LiveCharts2 Integration**: Replaced legacy `LiveCharts.Wpf` v0.9.7 with hardware-accelerated `LiveChartsCore.SkiaSharpView.WPF` v2.0.0-rc5.1 powered by SkiaSharp.
- **Custom Angular Gauge**: Engineered a pixel-perfect, pure-WPF angular gauge using geometric `<Path>` shapes and `<RotateTransform>`.
- **Async Data Streaming**: `TelemetryService` streams data using `IAsyncEnumerable<T>`, yielding results asynchronously over a 50ms interval.
- **CI/CD Pipeline**: GitHub Actions workflows for CI (`ci.yml`), CodeQL security scanning (`codeql.yml`), dependency review (`dependency-review.yml`), and automated releases (`release.yml`).
- **Dependabot**: Automated dependency management via `dependabot.yml`.
- **Global Error Handling**: Unhandled exception logging for App, TaskScheduler, and CurrentDomain.
- **Documentation**: `ARCHITECTURE.md` with Mermaid diagrams, `CONTRIBUTING.md`, `SECURITY.md`, issue/PR templates, and a professional `README.md` with CI badges.

### Fixed
- Fixed CSV parsing failures in non-English locales by enforcing `CultureInfo.InvariantCulture`.
- Resolved `DirectoryNotFoundException` crashes by mapping file paths to `AppDomain.CurrentDomain.BaseDirectory`.
- Fixed charting precision loss (empty charts) by converting timestamps from raw Ticks to normalized `TotalSeconds`.
- Fixed Start/Stop toggle unresponsiveness by decoupling state updates from `Task`-based blocking commands.

### Removed
- Legacy `packages.config` system eliminated in favor of `PackageReference`.
- Unused `.vs`, `obj`, and `bin` artifacts permanently removed from tracking via `.gitignore`.

## [0.1.0] - 2018-01-01

### Added
- Initial project release using .NET Framework 4.6.1.
- Real-time simulation dashboard with sliding-window telemetry for Pulse, Efficiency, Red, Green, and Blue series.
- Visual components modeled after factory angular gauges and Cartesian charts.
