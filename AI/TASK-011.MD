NOTE FOR CLAUDE CODE: Do not make assumptions, mock data, or placeholders. Always implement production-ready code.
GLOBAL UPGRADE REQUIREMENTS
- Upgrade all NuGet packages to the latest stable versions compatible with .NET 8.
- Migrate all projects to .NET 8 (net8.0) and update code for any breaking changes.
- Update the Discord API library to the latest stable version.
- Replace prefix commands with Discord slash commands and update parsing, modules, and documentation accordingly.

# TASK-011 - Configuration System Hardening

## Source
- CLAUDE-REPORT section 11 (Configuration System)

## Objective
- Ensure configuration sources are correctly layered and secrets are not stored in code.

## Tasks
1. Confirm JSON config is loaded in correct order with environment overrides.
2. Add typed options validation for required settings (Discord token, connection string).
3. Document required configuration keys and their sources.

## Implementation Notes
- Configuration loading is in `nhitomi/Startup.cs`.

## Code Snippet
```csharp
config
   .SetBasePath(host.HostingEnvironment.ContentRootPath)
   .AddJsonFile("appsettings.json")
   .AddJsonFile($"appsettings.{host.HostingEnvironment.EnvironmentName}.json", true)
   .AddEnvironmentVariables();
```

## Acceptance Criteria
- Missing required configuration fails fast with clear errors.
- Secrets are sourced from environment variables or secure stores.

