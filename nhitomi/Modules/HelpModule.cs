using System.Diagnostics;
using System.Runtime.InteropServices;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using nhitomi.Discord;
using nhitomi.Interactivity;

namespace nhitomi.Modules;

[RateLimit]
public class HelpModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly InteractiveManager _interactive;
    private readonly GuildSettingsCache _guildSettings;

    public HelpModule(
        InteractiveManager interactive,
        GuildSettingsCache guildSettings)
    {
        _interactive = interactive;
        _guildSettings = guildSettings;
    }

    private IDiscordContext CreateContext() => new SlashCommandContextAdapter(Context, _guildSettings);

    [SlashCommand("help", "Show help information about the bot")]
    public async Task HelpAsync()
    {
        await DeferAsync();
        await _interactive.SendInteractiveAsync(new HelpMessage(), CreateContext());
    }

    [SlashCommand("debug", "Show debug and diagnostic information")]
    public async Task DebugAsync()
    {
        await DeferAsync();
        await _interactive.SendInteractiveAsync(new DebugMessage(), CreateContext());
    }

    internal sealed class DebugMessage : EmbedMessage<DebugMessage.View>
    {
        public class View : ViewBase
        {
            private readonly DiscordService _discord;
            private readonly MessageHandlerService _messageHandler;
            private readonly ReactionHandlerService _reactionHandler;
            private readonly InteractiveManager _interactive;
            private readonly FeedChannelUpdateService _feedChannelUpdater;

            public View(
                DiscordService discord,
                MessageHandlerService messageHandler,
                ReactionHandlerService reactionHandler,
                InteractiveManager interactive,
                FeedChannelUpdateService feedChannelUpdater)
            {
                _discord = discord;
                _messageHandler = messageHandler;
                _reactionHandler = reactionHandler;
                _interactive = interactive;
                _feedChannelUpdater = feedChannelUpdater;
            }

            private sealed class ProcessMemory
            {
                public readonly long Virtual;
                public readonly long WorkingSet;
                public readonly long Managed;

                private const long Mebibytes = 1024 * 1024;

                public ProcessMemory()
                {
                    using var process = Process.GetCurrentProcess();
                    Virtual = process.VirtualMemorySize64 / Mebibytes;
                    WorkingSet = process.WorkingSet64 / Mebibytes;
                    Managed = GC.GetTotalMemory(false) / Mebibytes;
                }
            }

            public override async Task<bool> UpdateAsync(CancellationToken cancellationToken = default)
            {
                var memory = new ProcessMemory();
                var client = _discord.Client;

                var embed = new EmbedBuilder()
                    .WithTitle("**nhitomi**: Debug information")
                    .WithFields(
                        new EmbedFieldBuilder()
                            .WithName("Discord")
                            .WithValue($@"
Guilds: {client.Guilds.Count} guilds
Channels: {client.Guilds.Sum(g => g.TextChannels.Count) + client.PrivateChannels.Count} channels
Users: {client.Guilds.Sum(g => g.MemberCount)} users
Feed channels: {_feedChannelUpdater.UpdaterTasks.Count} updater tasks
Latency: {client.Latency}ms
Handled messages: {_messageHandler.HandledMessages} messages ({_messageHandler.ReceivedMessages} received)
Handled reactions: {_reactionHandler.HandledReactions} reactions ({_reactionHandler.ReceivedReactions} received)
Interactive messages: {_interactive.InteractiveMessages.Count} messages
Interactive triggers: {_interactive.InteractiveMessages.Sum(m => m.Value.Triggers.Count)} triggers
".Trim()),
                        new EmbedFieldBuilder()
                            .WithName("Process")
                            .WithValue($@"
Virtual memory: {memory.Virtual}MiB
Working set memory: {memory.WorkingSet}MiB
Managed memory: {memory.Managed}MiB
".Trim()),
                        new EmbedFieldBuilder()
                            .WithName("Runtime")
                            .WithValue($@"
{RuntimeInformation.OSDescription} {RuntimeInformation.OSArchitecture}
{RuntimeInformation.FrameworkDescription}
".Trim()))
                    .Build();

                await SetEmbedAsync(embed, cancellationToken);
                return true;
            }
        }
    }
}
