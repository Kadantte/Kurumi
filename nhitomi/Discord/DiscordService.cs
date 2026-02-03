using System.Collections.Concurrent;
using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using nhitomi.Core;
using nhitomi.Globalization;

namespace nhitomi.Discord;

public interface IDiscordContext
{
    IDiscordClient Client { get; }
    IUserMessage? Message { get; }
    IMessageChannel Channel { get; }
    IUser User { get; }
    Guild? GuildSettings { get; }
}

public interface ISlashCommandContext : IDiscordContext
{
    SocketInteraction Interaction { get; }
}

public class DiscordContextWrapper : IDiscordContext
{
    private readonly IDiscordContext? _context;

    public DiscordContextWrapper(IDiscordContext? context)
    {
        _context = context;
    }

    private IDiscordClient? _client;
    private IUserMessage? _message;
    private IMessageChannel? _channel;
    private IUser? _user;
    private Guild? _guild;

    public IDiscordClient Client
    {
        get => _client ?? _context?.Client!;
        set => _client = value;
    }

    public IUserMessage? Message
    {
        get => _message ?? _context?.Message;
        set => _message = value;
    }

    public IMessageChannel Channel
    {
        get => _channel ?? _context?.Channel!;
        set => _channel = value;
    }

    public IUser User
    {
        get => _user ?? _context?.User!;
        set => _user = value;
    }

    public Guild? GuildSettings
    {
        get => _guild ?? _context?.GuildSettings;
        set => _guild = value;
    }
}

public class DiscordService
{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _interactions;
    private readonly IServiceProvider _services;
    private readonly AppSettings _settings;
    private readonly ILogger<DiscordService> _logger;

    public DiscordSocketClient Client => _client;
    public InteractionService Interactions => _interactions;

    public DiscordService(
        DiscordSocketClient client,
        InteractionService interactions,
        IServiceProvider services,
        IOptions<AppSettings> options,
        ILogger<DiscordService> logger)
    {
        _client = client;
        _interactions = interactions;
        _services = services;
        _settings = options.Value;
        _logger = logger;

        _client.Ready += OnReady;
        _client.InteractionCreated += OnInteractionCreated;
    }

    private readonly ConcurrentQueue<TaskCompletionSource<object?>> _readyQueue = new();

    private Task OnReady()
    {
        while (_readyQueue.TryDequeue(out var source))
            source.TrySetResult(null);

        return Task.CompletedTask;
    }

    private async Task OnInteractionCreated(SocketInteraction interaction)
    {
        try
        {
            var ctx = new SocketInteractionContext(_client, interaction);
            await _interactions.ExecuteCommandAsync(ctx, _services);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling interaction");

            if (interaction.Type == InteractionType.ApplicationCommand)
            {
                try
                {
                    var response = await interaction.GetOriginalResponseAsync();
                    await response.DeleteAsync();
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
        }
    }

    public async Task ConnectAsync()
    {
        if (_client.LoginState != LoginState.LoggedOut || string.IsNullOrEmpty(_settings.Discord.Token))
            return;

        await _client.LoginAsync(TokenType.Bot, _settings.Discord.Token);
        await _client.StartAsync();

        _logger.LogInformation("Discord client connected");
    }

    public async Task RegisterCommandsAsync()
    {
        await WaitForReadyAsync();

        // Add modules from assembly
        await _interactions.AddModulesAsync(Assembly.GetExecutingAssembly(), _services);

        // Register commands globally or to test guild
        if (_settings.Discord.TestGuildId.HasValue)
        {
            await _interactions.RegisterCommandsToGuildAsync(_settings.Discord.TestGuildId.Value);
            _logger.LogInformation("Registered slash commands to test guild {GuildId}", _settings.Discord.TestGuildId.Value);
        }
        else
        {
            await _interactions.RegisterCommandsGloballyAsync();
            _logger.LogInformation("Registered slash commands globally");
        }
    }

    public async Task WaitForReadyAsync(CancellationToken cancellationToken = default)
    {
        if (_client.ConnectionState == ConnectionState.Connected)
            return;

        var source = new TaskCompletionSource<object?>();
        _readyQueue.Enqueue(source);

        using (cancellationToken.Register(() => source.TrySetCanceled()))
            await source.Task;
    }

    public async Task DisconnectAsync()
    {
        await _client.StopAsync();
        await _client.LogoutAsync();
    }
}

public static class DiscordContextExtensions
{
    public static Localization GetLocalization(this IDiscordContext context) =>
        Localization.GetLocalization(context.GuildSettings?.Language);

    public static IDisposable BeginTyping(this IDiscordContext context) =>
        context.Channel.EnterTypingState();

    public static async Task ReplyAsync(
        this IDiscordContext context,
        IMessageChannel channel,
        string localizationKey,
        object? variables = null,
        TimeSpan? expiry = null)
    {
        var message = await channel.SendMessageAsync(context.GetLocalization()[localizationKey, variables]);

        if (expiry != null)
        {
            _ = Task.Run(async () =>
            {
                await Task.Delay(expiry.Value);
                try
                {
                    await message.DeleteAsync();
                }
                catch
                {
                    // Ignore expiry exceptions
                }
            });
        }
    }

    public static Task ReplyAsync(
        this IDiscordContext context,
        string localizationKey,
        object? variables = null,
        TimeSpan? expiry = null) =>
        context.ReplyAsync(context.Channel, localizationKey, variables, expiry);

    public static async Task ReplyDmAsync(
        this IDiscordContext context,
        string localizationKey,
        object? variables = null,
        TimeSpan? expiry = null)
    {
        var dmChannel = await context.User.CreateDMChannelAsync();
        await context.ReplyAsync(dmChannel, localizationKey, variables, expiry);
    }
}
