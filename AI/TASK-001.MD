NOTE FOR CLAUDE CODE: Do not make assumptions, mock data, or placeholders. Always implement production-ready code.
GLOBAL UPGRADE REQUIREMENTS
- Upgrade all NuGet packages to the latest stable versions compatible with .NET 8.
- Migrate all projects to .NET 8 (net8.0) and update code for any breaking changes.
- Update the Discord API library to the latest stable version.
- Replace prefix commands with Discord slash commands and update parsing, modules, and documentation accordingly.

# TASK-001 - Executive Summary Alignment

## Source
- CLAUDE-REPORT section 1 (Executive Summary)

## Objective
- Align high-level project metadata (name, version, codename, license, purpose) across code and documentation.

## Tasks
1. Verify version and codename come from assembly metadata and match documentation.
2. Ensure MIT license attribution matches repository headers and README.
3. Confirm supported sources list matches implemented clients.

## Implementation Notes
- Version is defined in `nhitomi/nhitomi.csproj` and exposed via `nhitomi/VersionHelper.cs`.
- Supported sources are implemented under `nhitomi.Core/Clients`.

## Code Snippet
```csharp
public static Version Version => Assembly.GetName().Version;
public static string Codename =>
    Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            .InformationalVersion;
```

## Acceptance Criteria
- README and nhitomi_README.md show version 3.4 and codename Heresta.
- License text and copyright notice are consistent.
- Documentation lists only nhentai and hitomi as active sources.

