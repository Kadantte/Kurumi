using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using nhitomi.Core;
using nhitomi.Globalization;

namespace nhitomi.Discord
{
    public class GuildWelcomeMessageService : BackgroundService
    {
        readonly AppSettings _settings;
        readonly DiscordService _discord;
        readonly DiscordErrorReporter _errorReporter;

        public GuildWelcomeMessageService(IOptions<AppSettings> options,
                                          DiscordService discord,
                                          DiscordErrorReporter errorReporter)
        {
            _settings      = options.Value;
            _discord       = discord;
            _errorReporter = errorReporter;

            _discord.Client.JoinedGuild += HandleJoinedGuild;
        }

        async Task HandleJoinedGuild(SocketGuild guild)
        {
            try
            {
                // use default localization
                var l = Localization.Default["welcomeMessage"];

                var content = $@"{l["text"]}

**|** Use `/doujin get` to view a specific doujin
**|** Use `/doujin download` to get a download link
**|** Use `/doujin search` to search for doujins
**|** Use `/settings language` to change bot language

Use `/help` for more commands.

{l["openSource", new { repoUrl = "https://github.com/chiyadev/nhitomi" }]}";

                foreach (var channel in guild.TextChannels.OrderBy(c => c.Position))
                {
                    var perms = guild.CurrentUser.GetPermissions(channel);

                    // first channel where we can send messages
                    if (perms.SendMessages)
                    {
                        await channel.SendMessageAsync(content);
                        return;
                    }
                }

                // no channel to send messages
                // send to owner
                await guild.Owner.SendMessageAsync($@"
{content}
".Trim());
            }
            catch (Exception e)
            {
                await _errorReporter.ReportAsync(
                    e,
                    new GuildJoinedContext
                    {
                        Client  = _discord.Client,
                        Channel = guild.DefaultChannel,
                        User    = guild.Owner
                    },
                    false);
            }
        }

        class GuildJoinedContext : IDiscordContext
        {
            public IDiscordClient Client { get; set; }
            public IUserMessage? Message => null;
            public IMessageChannel Channel { get; set; }
            public IUser User { get; set; }
            public Guild? GuildSettings => new Guild();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.CompletedTask;
    }
}
