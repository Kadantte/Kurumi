NOTE FOR CLAUDE CODE: Do not make assumptions, mock data, or placeholders. Always implement production-ready code.
GLOBAL UPGRADE REQUIREMENTS
- Upgrade all NuGet packages to the latest stable versions compatible with .NET 8.
- Migrate all projects to .NET 8 (net8.0) and update code for any breaking changes.
- Update the Discord API library to the latest stable version.
- Replace prefix commands with Discord slash commands and update parsing, modules, and documentation accordingly.

# TASK-007 - Core Components Hardening

## Source
- CLAUDE-REPORT section 7 (Core Components)

## Objective
- Ensure entry point, startup configuration, and core services are reliable and production-ready.

## Tasks
1. Review host initialization and environment selection for correctness.
2. Validate database migration behavior for Development only.
3. Ensure Discord connection starts only after required initialization.

## Implementation Notes
- Entrypoint is `nhitomi/Program.cs` and host configuration is `nhitomi/Startup.cs`.

## Code Snippet
```csharp
using (var host = new HostBuilder()
                     .UseContentRoot(Environment.CurrentDirectory)
                     .UseEnvironment(Environment.GetEnvironmentVariable("ENVIRONMENT") ??
                                     EnvironmentName.Development)
                     .ConfigureAppConfiguration(Startup.Configure)
                     .ConfigureServices(Startup.ConfigureServices)
                     .Build())
{
    using (var scope = host.Services.CreateScope())
        await DependencyUtility<Initialization>.Factory(scope.ServiceProvider).RunAsync();

    await host.RunAsync();
}
```

## Acceptance Criteria
- Startup sequence runs without race conditions or null service resolution.
- Development-only migrations are gated correctly.

