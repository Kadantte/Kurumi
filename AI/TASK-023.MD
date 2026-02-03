NOTE FOR CLAUDE CODE: Do not make assumptions, mock data, or placeholders. Always implement production-ready code.
GLOBAL UPGRADE REQUIREMENTS
- Upgrade all NuGet packages to the latest stable versions compatible with .NET 8.
- Migrate all projects to .NET 8 (net8.0) and update code for any breaking changes.
- Update the Discord API library to the latest stable version.
- Replace prefix commands with Discord slash commands and update parsing, modules, and documentation accordingly.

# TASK-023 - Environment Variable Mapping

## Source
- CLAUDE-REPORT appendix C (Environment Variables)

## Objective
- Ensure environment variables are fully documented and mapped to configuration.

## Tasks
1. Document required environment variables in README and deployment docs.
2. Verify configuration binding to `AppSettings` for all required keys.
3. Add startup validation to fail fast when required values are missing.

## Implementation Notes
- Configuration is bound in `nhitomi/Startup.cs`.

## Code Snippet
```csharp
services
   .Configure<AppSettings>(host.Configuration);
```

## Acceptance Criteria
- Environment variable mapping is documented and correct.
- Missing required variables produce clear startup errors.

