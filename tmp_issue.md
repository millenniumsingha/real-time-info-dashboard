## Summary

Clean up the old .NET Framework project files and create a fresh .NET 10 solution file targeting the new `src/Dashboard/` project.

## Scope

- [ ] Create `RealTimeInfoDashboard.sln` at the root, referencing `src/Dashboard/Dashboard.csproj`.
- [ ] Delete legacy `.sln` (`Visualization.sln`).
- [ ] Delete old project files (`Dashboard/Dashboard.csproj`, `Dashboard/packages.config`, `Data/Data.csproj`).
- [ ] Delete remaining legacy directories (`Dashboard/` and `Data/`).
- [ ] Verify `dotnet build` succeeds across the new solution.

## Acceptance Criteria

- The legacy directories `Dashboard/` and `Data/` are completely removed.
- Valid `RealTimeInfoDashboard.sln` exists.
- Running `dotnet build` at the root successfully builds `src/Dashboard/Dashboard.csproj`.

## Context

- **Parent phase**: `phase/v1.0.0`
- **Related issues**: #19
