# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Comprehensive architecture documentation (`ARCHITECTURE.md`)
- Official changelog (`CHANGELOG.md`)

## [2.0.0] - 2026-02-25

### Added
- **.NET 10 Migration**: Upgraded the entire project from legacy .NET Framework to modern .NET 10 SDK-style projects.
- **MVVM Architecture**: Introduced `CommunityToolkit.Mvvm` and Microsoft Dependency Injection for a clean separation of concerns.
- **CI/CD Integration**: Added GitHub Actions workflow (`ci.yml`) for automated builds on Windows and automated dependency management via `dependabot.yml`.
- **Global Error Handling**: Added unhandled exception logging for App, TaskScheduler, and CurrentDomain to ensure robust telemetry on crashes.

### Changed
- **Dependency Update**: Migrated from legacy `LiveCharts.Wpf` v0.9.7 to modern `LiveChartsCore.SkiaSharpView.WPF` v2.0.0-rc5.1.
- Project restructuring: Unified the legacy `Data` and `Dashboard` projects into a single clean `src/Dashboard` directory.
- `TelemetryService` now streams data using `IAsyncEnumerable<T>`, yielding results asynchronously over a 50ms interval rather than spinning a blocking thread.

### Fixed
- Fixed an issue in `FactoryTelemetry` CSV parsing where parsing failed silently in locales that use commas for decimals by enforcing `CultureInfo.InvariantCulture`.
- Resolved `DirectoryNotFoundException` crashes when reading the telemetry CSV by mapping file paths to `AppDomain.CurrentDomain.BaseDirectory`.
- Fixed charting precision loss (empty charts) by converting timestamps from raw Ticks to normalized `TotalSeconds`.
- Fixed the Start/Stop toggle button unresponsiveness by decoupling state updates from `Task`-based blocking commands using synchronous fire-and-forget.

### Removed
- Legacy `<package id="..."/>` `packages.config` system eliminated in favor of `PackageReference`.
- Unused `.vs`, `obj`, and `bin` artifacts permanently removed from tracking via `.gitignore`.

## [1.0.0] - 2018-01-01

### Added
- Initial project release using .NET Framework 4.6.1.
- Real-time simulation dashboard with sliding-window telemetry for Pulse, Efficiency, Red, Green, and Blue series.
- Visual components modeled after factory angular gauges and Cartesian charts.
