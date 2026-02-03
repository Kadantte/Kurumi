NOTE FOR CLAUDE CODE: Do not make assumptions, mock data, or placeholders. Always implement production-ready code.
GLOBAL UPGRADE REQUIREMENTS
- Upgrade all NuGet packages to the latest stable versions compatible with .NET 8.
- Migrate all projects to .NET 8 (net8.0) and update code for any breaking changes.
- Update the Discord API library to the latest stable version.
- Replace prefix commands with Discord slash commands and update parsing, modules, and documentation accordingly.

# TASK-015 - Error Handling and Monitoring Enhancements

## Source
- CLAUDE-REPORT section 15 (Error Handling and Monitoring)

## Objective
- Ensure exceptions are reported with context and observability is production-grade.

## Tasks
1. Audit all try/catch blocks to confirm proper error reporting.
2. Add structured logging with category and event IDs.
3. Ensure debug metrics are accurate and thread-safe.

## Implementation Notes
- Message handling and error reporting are in `nhitomi/Discord/MessageHandlerService.cs`.

## Code Snippet
```csharp
try
{
    foreach (var handler in _messageHandlers)
    {
        if (await handler.TryHandleAsync(context))
        {
            HandledMessages.Increment();
            break;
        }
    }
}
catch (Exception e)
{
    await _errorReporter.ReportAsync(e, context);
}
```

## Acceptance Criteria
- Unhandled exceptions are logged and reported to the error channel.
- Debug metrics reflect real runtime values.

