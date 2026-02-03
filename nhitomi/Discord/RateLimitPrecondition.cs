using Discord;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;

namespace nhitomi.Discord;

/// <summary>
/// Precondition that enforces rate limiting on slash commands.
/// </summary>
public class RateLimitAttribute : PreconditionAttribute
{
    public override async Task<PreconditionResult> CheckRequirementsAsync(
        IInteractionContext context,
        ICommandInfo commandInfo,
        IServiceProvider services)
    {
        var rateLimiter = services.GetRequiredService<RateLimitService>();

        // Check user rate limit
        if (rateLimiter.IsUserRateLimited(context.User.Id, out var userRetry))
        {
            return PreconditionResult.FromError(
                $"You are being rate limited. Please wait {userRetry.TotalSeconds:F0} seconds.");
        }

        // Check guild rate limit if in a guild
        if (context.Guild != null && rateLimiter.IsGuildRateLimited(context.Guild.Id, out var guildRetry))
        {
            return PreconditionResult.FromError(
                $"This server is being rate limited. Please wait {guildRetry.TotalSeconds:F0} seconds.");
        }

        // Record the command execution
        rateLimiter.RecordUserCommand(context.User.Id);
        if (context.Guild != null)
        {
            rateLimiter.RecordGuildCommand(context.Guild.Id);
        }

        return PreconditionResult.FromSuccess();
    }
}

/// <summary>
/// Precondition that requires the user to have guild admin permissions.
/// </summary>
public class RequireGuildAdminAttribute : PreconditionAttribute
{
    public override Task<PreconditionResult> CheckRequirementsAsync(
        IInteractionContext context,
        ICommandInfo commandInfo,
        IServiceProvider services)
    {
        if (context.User is not IGuildUser user)
        {
            return Task.FromResult(PreconditionResult.FromError(
                "This command can only be used in a server."));
        }

        if (!user.GuildPermissions.ManageGuild)
        {
            return Task.FromResult(PreconditionResult.FromError(
                "You need the 'Manage Server' permission to use this command."));
        }

        return Task.FromResult(PreconditionResult.FromSuccess());
    }
}
