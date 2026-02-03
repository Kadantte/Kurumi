NOTE FOR CLAUDE CODE: Do not make assumptions, mock data, or placeholders. Always implement production-ready code.
GLOBAL UPGRADE REQUIREMENTS
- Upgrade all NuGet packages to the latest stable versions compatible with .NET 8.
- Migrate all projects to .NET 8 (net8.0) and update code for any breaking changes.
- Update the Discord API library to the latest stable version.
- Replace prefix commands with Discord slash commands and update parsing, modules, and documentation accordingly.

# TASK-020 - Recommendations Roadmap Implementation

## Source
- CLAUDE-REPORT section 20 (Recommendations)

## Objective
- Convert recommendations into actionable, production-ready roadmap items.

## Tasks
1. Upgrade to supported .NET and update all dependent libraries.
2. Add caching for frequent reads (doujin, tags, guild settings).
3. Add health checks and operational metrics endpoints.
4. Evaluate queue-based feed delivery for scalability.

## Implementation Notes
- Use Microsoft.Extensions.Caching.Memory for in-process caching as a baseline.

## Code Snippet
```csharp
services.AddMemoryCache();

public class GuildSettingsCache
{
    private readonly IMemoryCache _cache;

    public GuildSettingsCache(IMemoryCache cache)
    {
        _cache = cache;
    }
}
```

## Acceptance Criteria
- Roadmap items are prioritized and tracked with owners and milestones.
- Caching improves repeated read latency without stale data regressions.

