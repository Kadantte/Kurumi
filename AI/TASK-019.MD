NOTE FOR CLAUDE CODE: Do not make assumptions, mock data, or placeholders. Always implement production-ready code.
GLOBAL UPGRADE REQUIREMENTS
- Upgrade all NuGet packages to the latest stable versions compatible with .NET 8.
- Migrate all projects to .NET 8 (net8.0) and update code for any breaking changes.
- Update the Discord API library to the latest stable version.
- Replace prefix commands with Discord slash commands and update parsing, modules, and documentation accordingly.

# TASK-019 - Security Controls and Permissions

## Source
- CLAUDE-REPORT section 19 (Security Considerations)

## Objective
- Enforce permissions, reduce abuse, and protect secrets.

## Tasks
1. Implement per-user and per-guild command rate limiting.
2. Audit admin-only commands and ensure permission checks are applied.
3. Enforce secret loading from environment variables or secure stores only.

## Implementation Notes
- Admin validation exists in `nhitomi/Modules/OptionModule.cs`.

## Code Snippet
```csharp
public static async Task<bool> EnsureGuildAdminAsync(IDiscordContext context,
                                                     CancellationToken cancellationToken = default)
{
    if (!(context.User is IGuildUser user))
    {
        await context.ReplyAsync("commandInvokeNotInGuild");
        return false;
    }

    if (!user.GuildPermissions.ManageGuild)
    {
        await context.ReplyAsync("notGuildAdmin");
        return false;
    }

    return true;
}
```

## Acceptance Criteria
- Admin-only commands are inaccessible to non-admin users.
- Rate limits prevent command abuse without blocking legitimate usage.
- Tokens are never stored in source-controlled files.

