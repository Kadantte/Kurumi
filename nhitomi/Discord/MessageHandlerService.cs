using System.Diagnostics.CodeAnalysis;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using nhitomi.Core;
using nhitomi.Interactivity;

namespace nhitomi.Discord;

public interface IMessageHandler
{
    Task InitializeAsync(CancellationToken cancellationToken = default);

    Task<bool> TryHandleAsync(IMessageContext context,
                              CancellationToken cancellationToken = default);
}

public interface IMessageContext : IDiscordContext
{
    MessageEvent Event { get; }
}

public enum MessageEvent
{
    Create,
    Modify,
    Delete
}

public class MessageHandlerService : IHostedService
{
    private readonly DiscordService _discord;
    private readonly GuildSettingsCache _guildSettingsCache;
    private readonly DiscordErrorReporter _errorReporter;
    private readonly ILogger<MessageHandlerService> _logger;

    private readonly IMessageHandler[] _messageHandlers;

    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
    public MessageHandlerService(
        DiscordService discord,
        GuildSettingsCache guildSettingsCache,
        DiscordErrorReporter errorReporter,
        ILogger<MessageHandlerService> logger,
        GalleryUrlDetector galleryUrlDetector,
        InteractiveManager interactiveManager)
    {
        _discord = discord;
        _guildSettingsCache = guildSettingsCache;
        _errorReporter = errorReporter;
        _logger = logger;

        // Note: CommandExecutor removed - now using slash commands via DiscordService
        _messageHandlers =
        [
            galleryUrlDetector,
            interactiveManager
        ];
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _discord.WaitForReadyAsync(cancellationToken);

        await Task.WhenAll(_messageHandlers.Select(h => h.InitializeAsync(cancellationToken)));

        var client = _discord.Client;
        client.MessageReceived += MessageReceived;
        client.MessageUpdated += MessageUpdated;
        client.MessageDeleted += MessageDeleted;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        var client = _discord.Client;
        client.MessageReceived -= MessageReceived;
        client.MessageUpdated -= MessageUpdated;
        client.MessageDeleted -= MessageDeleted;

        return Task.CompletedTask;
    }

    private Task MessageReceived(SocketMessage message) =>
        HandleMessageAsync(message, MessageEvent.Create);

    private Task MessageUpdated(
        Cacheable<IMessage, ulong> _,
        SocketMessage message,
        ISocketMessageChannel channel) =>
        HandleMessageAsync(message, MessageEvent.Modify);

    private Task MessageDeleted(
        Cacheable<IMessage, ulong> cacheable,
        Cacheable<IMessageChannel, ulong> channel)
    {
        if (cacheable.HasValue)
            return HandleMessageAsync(cacheable.Value, MessageEvent.Delete);

        return Task.CompletedTask;
    }

    public readonly AtomicCounter HandledMessages = new();
    public readonly AtomicCounter ReceivedMessages = new();

    private Task HandleMessageAsync(IMessage socketMessage, MessageEvent eventType)
    {
        if (socketMessage is IUserMessage message &&
            !socketMessage.Author.IsBot &&
            !socketMessage.Author.IsWebhook)
        {
            _ = Task.Run(async () =>
            {
                var context = new MessageContext
                {
                    Client = _discord.Client,
                    Message = message,
                    Event = eventType,
                    GuildSettings = _guildSettingsCache[message.Channel]
                };

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
                finally
                {
                    ReceivedMessages.Increment();
                }
            });
        }

        return Task.CompletedTask;
    }

    private class MessageContext : IMessageContext
    {
        public required IDiscordClient Client { get; set; }
        public required IUserMessage Message { get; set; }
        public IMessageChannel Channel => Message.Channel;
        public IUser User => Message.Author;
        public Guild? GuildSettings { get; set; }

        public MessageEvent Event { get; set; }
    }
}
