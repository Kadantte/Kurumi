NOTE FOR CLAUDE CODE: Do not make assumptions, mock data, or placeholders. Always implement production-ready code.
GLOBAL UPGRADE REQUIREMENTS
- Upgrade all NuGet packages to the latest stable versions compatible with .NET 8.
- Migrate all projects to .NET 8 (net8.0) and update code for any breaking changes.
- Update the Discord API library to the latest stable version.
- Replace prefix commands with Discord slash commands and update parsing, modules, and documentation accordingly.

# TASK-018 - Dependency Audit and Updates

## Source
- CLAUDE-REPORT section 18 (Dependencies)

## Objective
- Keep dependencies current and monitor for vulnerabilities.

## Tasks
1. Run dependency audit for all projects and record results.
2. Update packages to supported versions aligned with the .NET upgrade plan.
3. Configure Renovate to cover nuget dependencies if not already enabled.

## Implementation Notes
- Dependencies are defined in project csproj files.

## Code Snippet
```bash
dotnet list nhitomi.sln package --vulnerable
```

## Acceptance Criteria
- Audit results are captured and actionable.
- Dependency versions match supported frameworks.

