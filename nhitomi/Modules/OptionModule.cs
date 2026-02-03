using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Options;
using nhitomi.Core;
using nhitomi.Discord;
using nhitomi.Globalization;

namespace nhitomi.Modules;

[Group("settings", "Configure bot settings for this server")]
[RateLimit]
[RequireGuildAdmin]
public class OptionModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IDatabase _db;
    private readonly GuildSettingsCache _settingsCache;

    public OptionModule(
        IDatabase db,
        GuildSettingsCache settingsCache)
    {
        _db = db;
        _settingsCache = settingsCache;
    }

    [SlashCommand("language", "Set the bot language for this server")]
    public async Task LanguageAsync(
        [Summary("language", "Language code to set")]
        [Choice("English", "en")]
        [Choice("Indonesian", "id")]
        [Choice("Korean", "ko")]
        string language)
    {
        await DeferAsync();

        if (!Localization.IsAvailable(language))
        {
            await FollowupAsync($"Language '{language}' is not available.", ephemeral: true);
            return;
        }

        if (Context.Guild == null)
        {
            await FollowupAsync("This command can only be used in a server.", ephemeral: true);
            return;
        }

        Guild? guild;

        do
        {
            guild = await _db.GetGuildAsync(Context.Guild.Id);
            guild.Language = language;
        }
        while (!await _db.SaveAsync());

        _settingsCache[Context.Channel] = guild;

        var localization = Localization.GetLocalization(language);
        await FollowupAsync($"Language changed to {localization.Culture.EnglishName} ({language}).");
    }
}

[Group("feed", "Configure feed channels for automatic doujin updates")]
[RateLimit]
[RequireGuildAdmin]
public class FeedModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly AppSettings _settings;
    private readonly IDatabase _db;
    private readonly GuildSettingsCache _guildSettings;

    public FeedModule(
        IOptions<AppSettings> options,
        IDatabase db,
        GuildSettingsCache guildSettings)
    {
        _settings = options.Value;
        _db = db;
        _guildSettings = guildSettings;
    }

    private async Task<bool> EnsureFeedEnabled()
    {
        if (_settings.Feed.Enabled)
            return true;

        await RespondAsync("Feed channels are currently disabled.", ephemeral: true);
        return false;
    }

    [SlashCommand("add-tag", "Add a tag to the feed channel whitelist")]
    public async Task AddTagAsync(
        [Summary("tag", "Tag to add to the whitelist")] string tag)
    {
        if (!await EnsureFeedEnabled())
            return;

        await DeferAsync();

        var guildSettings = _guildSettings[Context.Channel];
        if (guildSettings == null)
        {
            await FollowupAsync("Could not retrieve guild settings.", ephemeral: true);
            return;
        }

        var added = false;

        do
        {
            var channel = await _db.GetFeedChannelAsync(
                guildSettings.Id,
                Context.Channel.Id);

            var tags = await _db.GetTagsAsync(tag);

            if (tags.Length == 0)
            {
                await FollowupAsync($"Tag '{tag}' not found.", ephemeral: true);
                return;
            }

            foreach (var t in tags)
            {
                var tagRef = channel.Tags.FirstOrDefault(x => x.TagId == t.Id);

                if (tagRef == null)
                {
                    channel.Tags.Add(new FeedChannelTag
                    {
                        Tag = t
                    });

                    added = true;
                }
            }
        }
        while (!await _db.SaveAsync());

        if (added)
            await FollowupAsync($"Added tag '{tag}' to feed channel.");
        else
            await FollowupAsync($"Tag '{tag}' is already in the feed channel whitelist.", ephemeral: true);
    }

    [SlashCommand("remove-tag", "Remove a tag from the feed channel whitelist")]
    public async Task RemoveTagAsync(
        [Summary("tag", "Tag to remove from the whitelist")] string tag)
    {
        if (!await EnsureFeedEnabled())
            return;

        await DeferAsync();

        var guildSettings = _guildSettings[Context.Channel];
        if (guildSettings == null)
        {
            await FollowupAsync("Could not retrieve guild settings.", ephemeral: true);
            return;
        }

        var removed = false;

        do
        {
            var channel = await _db.GetFeedChannelAsync(
                guildSettings.Id,
                Context.Channel.Id);

            foreach (var t in await _db.GetTagsAsync(tag))
            {
                var tagRef = channel.Tags.FirstOrDefault(x => x.TagId == t.Id);

                if (tagRef != null)
                {
                    channel.Tags.Remove(tagRef);
                    removed = true;
                }
            }
        }
        while (!await _db.SaveAsync());

        if (removed)
            await FollowupAsync($"Removed tag '{tag}' from feed channel.");
        else
            await FollowupAsync($"Tag '{tag}' was not in the feed channel whitelist.", ephemeral: true);
    }

    [SlashCommand("mode", "Set the feed channel matching mode")]
    public async Task ModeAsync(
        [Summary("mode", "How tags should be matched")]
        [Choice("Any (match any tag)", "any")]
        [Choice("All (match all tags)", "all")]
        string mode)
    {
        if (!await EnsureFeedEnabled())
            return;

        await DeferAsync();

        var guildSettings = _guildSettings[Context.Channel];
        if (guildSettings == null)
        {
            await FollowupAsync("Could not retrieve guild settings.", ephemeral: true);
            return;
        }

        if (!Enum.TryParse<FeedChannelWhitelistType>(mode, true, out var type))
        {
            await FollowupAsync("Invalid mode.", ephemeral: true);
            return;
        }

        do
        {
            var channel = await _db.GetFeedChannelAsync(
                guildSettings.Id,
                Context.Channel.Id);

            channel.WhitelistType = type;
        }
        while (!await _db.SaveAsync());

        await FollowupAsync($"Feed channel mode changed to '{type}'.");
    }
}
