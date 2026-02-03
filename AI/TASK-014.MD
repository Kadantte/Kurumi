NOTE FOR CLAUDE CODE: Do not make assumptions, mock data, or placeholders. Always implement production-ready code.
GLOBAL UPGRADE REQUIREMENTS
- Upgrade all NuGet packages to the latest stable versions compatible with .NET 8.
- Migrate all projects to .NET 8 (net8.0) and update code for any breaking changes.
- Update the Discord API library to the latest stable version.
- Replace prefix commands with Discord slash commands and update parsing, modules, and documentation accordingly.

# TASK-014 - Data Flow Instrumentation

## Source
- CLAUDE-REPORT section 14 (Data Flow Analysis)

## Objective
- Add tracing and measurable checkpoints to core flows (search, collection, feed).

## Tasks
1. Add structured logs with correlation IDs for search and collection flows.
2. Emit timing metrics for database queries and message rendering.
3. Document flow steps alongside code references.

## Implementation Notes
- Search flow originates in `nhitomi/Modules/DoujinModule.cs`.

## Code Snippet
```csharp
await _interactive.SendInteractiveAsync(
    new DoujinListFromQueryMessage(new DoujinSearchArgs
    {
        Query         = query,
        QualityFilter = false,
        Source        = GalleryUtility.ExpandContraction(source)
    }),
    _context,
    cancellationToken);
```

## Acceptance Criteria
- Key flows emit logs with timing and correlation data.
- Flow documentation matches actual code paths.

