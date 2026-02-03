NOTE FOR CLAUDE CODE: Do not make assumptions, mock data, or placeholders. Always implement production-ready code.
GLOBAL UPGRADE REQUIREMENTS
- Upgrade all NuGet packages to the latest stable versions compatible with .NET 8.
- Migrate all projects to .NET 8 (net8.0) and update code for any breaking changes.
- Update the Discord API library to the latest stable version.
- Replace prefix commands with Discord slash commands and update parsing, modules, and documentation accordingly.

# TASK-022 - Command Quick Reference Generation

## Source
- CLAUDE-REPORT appendix B (Command Quick Reference)

## Objective
- Generate command reference directly from code to keep docs accurate.

## Tasks
1. Build a command reference generator using reflection over command attributes.
2. Output the reference into README or a dedicated commands document.
3. Validate the output in CI.

## Implementation Notes
- Command attributes live in `nhitomi/Discord/Parsing` and are discovered in `CommandExecutor`.

## Code Snippet
```csharp
var commands = typeof(Startup).Assembly
    .GetTypes()
    .Where(t => t.IsClass && !t.IsAbstract)
    .SelectMany(t => t.GetMethods())
    .Where(m => m.GetCustomAttribute<CommandAttribute>() != null)
    .Select(m => new CommandInfo(m))
    .ToArray();
```

## Acceptance Criteria
- Command reference is generated from code and matches runtime behavior.
- CI fails if the generated reference is out of date.

