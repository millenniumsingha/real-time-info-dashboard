# Contributing to Real-Time Info Dashboard

Thank you for your interest in contributing to the **Real-Time Factory Dashboard**! This document outlines the required processes and architectural standards for all contributions to the repository.

## 🏗️ Core Architecture & Conventions

Before submitting code, please familiarize yourself with the project's foundational technologies:
- **MVVM Pattern**: The project strictly enforces Model-View-ViewModel using `CommunityToolkit.Mvvm`. Views (`.xaml` and `.xaml.cs`) must contain absolutely no business logic. State and logic reside purely in ViewModels.
- **Dependency Injection**: Services and ViewModels are injected via Microsoft's standard `IServiceCollection`. Do not instantiate ViewModels manually.
- **High-Performance Rendering**: We use `LiveChartsCore.SkiaSharpView.WPF` for hardware-accelerated charting. Cartesian charts operate on normalized `TotalSeconds` on the X-axis to prevent `double` precision loss that occurs with raw `DateTime.Ticks`.
- **Custom UI Overrides**: Advanced UI paradigms (like our custom Angular Gauge) rely on pure WPF primitives (Canvas, Path, Line, RotateTransform) instead of constrained charting controls.

## 🌿 Branching Strategy & Lifecycle

We use a strict **Phase-Based Milestone Workflow**. Do not branch from or merge directly to `master`.

1. **Active Integration Branch**: All feature development takes place against the current milestone branch (e.g., `phase/v1.0.0`).
2. **Issue Creation**: Every change requires a GitHub issue first. Use the provided Issue Templates.
3. **Feature Branches**: Branch exclusively from the active phase branch using the conventional naming format:
   `type/scope-issuenumber` (e.g., `feat/mvvm-setup-8` or `docs/architecture-changelog-11`)
4. **No Batching**: Issues and branches must be granular. Fix one scoped problem per branch.

## 📥 Submitting Changes

1. **Commit Messages**: We strictly adhere to [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/).
   Examples: 
   - `feat(charts): migrate to LiveCharts2 (closes #28)`
   - `docs: add CONTRIBUTING.md (closes #32)`
2. **Pull Requests**:
   - Set the PR base to the active phase branch (e.g., `phase/v1.0.0`), NOT `master`.
   - Use the provided PR Template to explain your 'What', 'Changes', and 'Testing' checklist.
   - All PRs must pass the exacted CI pipeline (`ci.yml` compiling to `net10.0-windows` targets).
3. **Merge Strategy**: PRs are merged via **Squash and Merge**.

## 🐛 Reporting Bugs

When reporting an issue, ensure you provide:
- The exact `.NET` SDK and Windows OS version you are running.
- If it's a render or chart crash, please attach the `crash_task.log` or `crash.log` file generated in the application's executable directory.

We appreciate all your efforts to make this dashboard as high-performance and robust as possible!
