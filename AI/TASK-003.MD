NOTE FOR CLAUDE CODE: Do not make assumptions, mock data, or placeholders. Always implement production-ready code.
GLOBAL UPGRADE REQUIREMENTS
- Upgrade all NuGet packages to the latest stable versions compatible with .NET 8.
- Migrate all projects to .NET 8 (net8.0) and update code for any breaking changes.
- Update the Discord API library to the latest stable version.
- Replace prefix commands with Discord slash commands and update parsing, modules, and documentation accordingly.

# TASK-003 - Technology Stack Audit and Upgrade Plan

## Source
- CLAUDE-REPORT section 3 (Technology Stack)

## Objective
- Inventory current frameworks and dependencies and plan a supported upgrade path.

## Tasks
1. Capture current target frameworks and package versions from csproj files.
2. Produce a migration plan to supported .NET (net8.0) and compatible dependencies.
3. Update CI and Dockerfile to use supported SDK/runtime images.

## Implementation Notes
- Target frameworks and packages are defined in `nhitomi/nhitomi.csproj`, `nhitomi.Core/nhitomi.Core.csproj`, and `nhitomi.Core.UnitTests/nhitomi.Core.UnitTests.csproj`.

## Code Snippet
```xml
<TargetFramework>netcoreapp2.2</TargetFramework>
<PackageReference Include="Discord.Net" Version="2.1.1" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="2.2.4" />
```

## Acceptance Criteria
- A written upgrade plan lists exact target versions and breaking changes.
- CI and Docker build the upgraded frameworks without using deprecated images.

