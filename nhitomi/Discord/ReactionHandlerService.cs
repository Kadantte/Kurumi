using System.Diagnostics.CodeAnalysis;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using nhitomi.Core;
using nhitomi.Interactivity;

namespace nhitomi.Discord;

public interface IReactionHandler
{
    Task InitializeAsync(CancellationToken cancellationToken = default);

    Task<bool> TryHandleAsync(IReactionContext context,
                              CancellationToken cancellationToken = default);
}

public interface IReactionContext : IDiscordContext
{
    IReaction Reaction { get; }
    ReactionEvent Event { get; }
}

public enum ReactionEvent
{
    Add,
    Remove
}

public class ReactionHandlerService : IHostedService
{
    private readonly DiscordService _discord;
    private readonly GuildSettingsCache _guildSettingsCache;
    private readonly DiscordErrorReporter _errorReporter;
    private readonly ILogger<ReactionHandlerService> _logger;

    private readonly IReactionHandler[] _reactionHandlers;

    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
    public ReactionHandlerService(
        DiscordService discord,
        GuildSettingsCache guildSettingsCache,
        DiscordErrorReporter errorReporter,
        ILogger<ReactionHandlerService> logger,
        InteractiveManager interactiveManager)
    {
        _discord = discord;
        _guildSettingsCache = guildSettingsCache;
        _errorReporter = errorReporter;
        _logger = logger;

        _reactionHandlers =
        [
            interactiveManager
        ];
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _discord.WaitForReadyAsync(cancellationToken);

        await Task.WhenAll(_reactionHandlers.Select(h => h.InitializeAsync(cancellationToken)));

        var client = _discord.Client;
        client.ReactionAdded += ReactionAdded;
        client.ReactionRemoved += ReactionRemoved;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        var client = _discord.Client;
        client.ReactionAdded -= ReactionAdded;
        client.ReactionRemoved -= ReactionRemoved;

        return Task.CompletedTask;
    }

    private Task ReactionAdded(
        Cacheable<IUserMessage, ulong> _,
        Cacheable<IMessageChannel, ulong> channel,
        SocketReaction reaction) =>
        HandleReactionAsync(channel, reaction, ReactionEvent.Add);

    private Task ReactionRemoved(
        Cacheable<IUserMessage, ulong> _,
        Cacheable<IMessageChannel, ulong> channel,
        SocketReaction reaction) =>
        HandleReactionAsync(channel, reaction, ReactionEvent.Remove);

    public readonly AtomicCounter HandledReactions = new();
    public readonly AtomicCounter ReceivedReactions = new();

    private Task HandleReactionAsync(
        Cacheable<IMessageChannel, ulong> channelCacheable,
        SocketReaction reaction,
        ReactionEvent eventType)
    {
        var currentUser = _discord.Client.CurrentUser;
        if (currentUser != null && reaction.UserId != currentUser.Id)
        {
            _ = Task.Run(async () =>
            {
                // Retrieve channel
                var channel = await channelCacheable.GetOrDownloadAsync();
                if (channel == null)
                    return;

                // Retrieve message
                if (await channel.GetMessageAsync(reaction.MessageId) is not IUserMessage message)
                    return;

                // Retrieve user
                if (await channel.GetUserAsync(reaction.UserId) is not IUser user)
                    return;

                // Create context
                var context = new ReactionContext
                {
                    Client = _discord.Client,
                    Message = message,
                    User = user,
                    GuildSettings = _guildSettingsCache[message.Channel],
                    Reaction = reaction,
                    Event = eventType
                };

                try
                {
                    foreach (var handler in _reactionHandlers)
                    {
                        if (await handler.TryHandleAsync(context))
                        {
                            HandledReactions.Increment();
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    await _errorReporter.ReportAsync(e, context, false);
                }
                finally
                {
                    ReceivedReactions.Increment();
                }
            });
        }

        return Task.CompletedTask;
    }

    private class ReactionContext : IReactionContext
    {
        public required IDiscordClient Client { get; set; }
        public required IUserMessage Message { get; set; }
        public IMessageChannel Channel => Message.Channel;
        public required IUser User { get; set; }
        public Guild? GuildSettings { get; set; }

        public required IReaction Reaction { get; set; }
        public ReactionEvent Event { get; set; }
    }
}
