using System.ComponentModel.DataAnnotations;
using Discord;
using Discord.WebSocket;

namespace nhitomi;

public sealed class AppSettings : IValidatableObject
{
    public DiscordSettings Discord { get; set; } = new();
    public HttpSettings Http { get; set; } = new();
    public FeedSettings Feed { get; set; } = new();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Discord.Token))
            yield return new ValidationResult("Discord:Token is required", new[] { nameof(Discord) });

        if (Http.TimeoutSeconds <= 0)
            yield return new ValidationResult("Http:TimeoutSeconds must be positive", new[] { nameof(Http) });

        if (Http.RetryCount < 0)
            yield return new ValidationResult("Http:RetryCount cannot be negative", new[] { nameof(Http) });

        if (Http.RetryDelayMilliseconds < 0)
            yield return new ValidationResult("Http:RetryDelayMilliseconds cannot be negative", new[] { nameof(Http) });
    }

    /// <summary>
    /// Validates the settings and throws if invalid.
    /// </summary>
    public void ValidateAndThrow()
    {
        var results = new List<ValidationResult>();
        if (!Validator.TryValidateObject(this, new ValidationContext(this), results, true))
        {
            var errors = string.Join(Environment.NewLine, results.Select(r => r.ErrorMessage));
            throw new InvalidOperationException($"Configuration validation failed:{Environment.NewLine}{errors}");
        }

        // Run IValidatableObject validation
        var customResults = Validate(new ValidationContext(this)).ToList();
        if (customResults.Count > 0)
        {
            var errors = string.Join(Environment.NewLine, customResults.Select(r => r.ErrorMessage));
            throw new InvalidOperationException($"Configuration validation failed:{Environment.NewLine}{errors}");
        }
    }

    public sealed class DiscordSettings
    {
        public string? Token { get; set; }
        public string? BotInvite { get; set; }
        public ulong? TestGuildId { get; set; }

        public StatusSettings Status { get; set; } = new();
        public GuildSettings Guild { get; set; } = new();

        public DiscordSocketConfig GetSocketConfig() => new()
        {
            GatewayIntents = GatewayIntents.Guilds |
                             GatewayIntents.GuildMessages |
                             GatewayIntents.GuildMessageReactions |
                             GatewayIntents.DirectMessages |
                             GatewayIntents.DirectMessageReactions |
                             GatewayIntents.MessageContent,
            AlwaysDownloadUsers = false,
            MessageCacheSize = 100,
            LogLevel = LogSeverity.Info
        };

        public sealed class StatusSettings
        {
            public double UpdateInterval { get; set; } = 60;
            public string[] Games { get; set; } = [];
        }

        public sealed class GuildSettings
        {
            public ulong GuildId { get; set; }
            public string? GuildInvite { get; set; }
            public ulong ErrorChannelId { get; set; }
        }
    }

    public sealed class HttpSettings
    {
        public bool EnableProxy { get; set; }
        public int TimeoutSeconds { get; set; } = 30;
        public int RetryCount { get; set; } = 3;
        public int RetryDelayMilliseconds { get; set; } = 500;
    }

    public sealed class FeedSettings
    {
        public bool Enabled { get; set; }
    }
}
