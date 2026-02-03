NOTE FOR CLAUDE CODE: Do not make assumptions, mock data, or placeholders. Always implement production-ready code.
GLOBAL UPGRADE REQUIREMENTS
- Upgrade all NuGet packages to the latest stable versions compatible with .NET 8.
- Migrate all projects to .NET 8 (net8.0) and update code for any breaking changes.
- Update the Discord API library to the latest stable version.
- Replace prefix commands with Discord slash commands and update parsing, modules, and documentation accordingly.

# TASK-013 - External Integrations and HTTP Policy

## Source
- CLAUDE-REPORT section 13 (External Integrations)

## Objective
- Harden HTTP usage and Discord integration boundaries with resilient policies.

## Tasks
1. Configure HttpClientFactory policies (timeouts, retries, and decompression) for all outbound calls.
2. Ensure per-source headers are set correctly (example: referrer for Hitomi images).
3. Add logging and metrics for external call failures.

## Implementation Notes
- HTTP wrapper is in `nhitomi.Core/HttpClientWrapper.cs`.
- Source-specific request setup is in each client.

## Code Snippet
```csharp
public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                           CancellationToken cancellationToken = default) =>
    Http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
```

## Acceptance Criteria
- All outbound HTTP calls apply consistent timeout and retry behavior.
- Source-specific headers are set where required.

