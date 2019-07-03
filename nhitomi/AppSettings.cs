using Discord.WebSocket;

namespace nhitomi
{
    public sealed class AppSettings
    {
        public DiscordSettings Discord { get; set; } = new DiscordSettings();

        public sealed class DiscordSettings : DiscordSocketConfig
        {
            public string Prefix { get; set; }
            public string Token { get; set; }
            public string BotInvite { get; set; }

            public StatusSettings Status { get; set; } = new StatusSettings();

            public sealed class StatusSettings
            {
                public double UpdateInterval { get; set; }
                public string[] Games { get; set; }
            }

            public GuildSettings Guild { get; set; } = new GuildSettings();

            public sealed class GuildSettings
            {
                public ulong GuildId { get; set; }
                public string GuildInvite { get; set; }
                public ulong ErrorChannelId { get; set; }
            }
        }

        public HttpSettings Http { get; set; } = new HttpSettings();

        public sealed class HttpSettings
        {
            public bool EnableProxy { get; set; }
        }

        public FeedSettings Feed { get; set; } = new FeedSettings();

        public sealed class FeedSettings
        {
            public bool Enabled { get; set; }
        }
    }
}