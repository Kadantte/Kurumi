NOTE FOR CLAUDE CODE: Do not make assumptions, mock data, or placeholders. Always implement production-ready code.
GLOBAL UPGRADE REQUIREMENTS
- Upgrade all NuGet packages to the latest stable versions compatible with .NET 8.
- Migrate all projects to .NET 8 (net8.0) and update code for any breaking changes.
- Update the Discord API library to the latest stable version.
- Replace prefix commands with Discord slash commands and update parsing, modules, and documentation accordingly.

# TASK-017 - File Inventory Automation

## Source
- CLAUDE-REPORT section 17 (File Inventory)

## Objective
- Automate file inventory generation and keep it consistent across reports.

## Tasks
1. Create a script that lists non-build files and categorizes them by type.
2. Save inventory output into AI reports on demand.
3. Ensure inventory excludes `obj` and `bin` directories.

## Implementation Notes
- A PowerShell-based inventory script fits the existing Windows environment.

## Code Snippet
```powershell
$root = Get-Location
Get-ChildItem -Recurse -File |
  Where-Object { $_.FullName -notmatch '\\obj\\' -and $_.FullName -notmatch '\\bin\\' } |
  ForEach-Object { $_.FullName.Substring($root.Path.Length + 1) } |
  Sort-Object
```

## Acceptance Criteria
- Inventory output is reproducible and excludes build artifacts.
- Output is stored in the AI folder when generated.

