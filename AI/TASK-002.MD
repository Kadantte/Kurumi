NOTE FOR CLAUDE CODE: Do not make assumptions, mock data, or placeholders. Always implement production-ready code.
GLOBAL UPGRADE REQUIREMENTS
- Upgrade all NuGet packages to the latest stable versions compatible with .NET 8.
- Migrate all projects to .NET 8 (net8.0) and update code for any breaking changes.
- Update the Discord API library to the latest stable version.
- Replace prefix commands with Discord slash commands and update parsing, modules, and documentation accordingly.

# TASK-002 - Project Overview and Command Coverage

## Source
- CLAUDE-REPORT section 2 (Project Overview)

## Objective
- Ensure project purpose and command set are accurately documented and verifiable from code.

## Tasks
1. Reconcile command lists in README and nhitomi_README.md with actual module definitions.
2. Add or update usage examples for each command that exists in code.
3. Verify aliases and bindings match the documented syntax.

## Implementation Notes
- Commands are defined with attributes in `nhitomi/Modules/*.cs` and loaded via `Discord/CommandExecutor.cs`.

## Code Snippet
```csharp
[Command("get", Alias = "g")]
public async Task GetAsync(string source,
                           string id,
                           CancellationToken cancellationToken = default)
```

## Acceptance Criteria
- Documentation commands, aliases, and syntax match the compiled command list.
- No documented command is missing in code, and no implemented command is undocumented.

