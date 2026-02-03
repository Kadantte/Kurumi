using Discord;
using Discord.Interactions;
using nhitomi.Core;
using nhitomi.Discord;
using nhitomi.Interactivity;

namespace nhitomi.Modules;

[Group("doujin", "Commands for browsing and interacting with doujinshi")]
[RateLimit]
public class DoujinModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IDatabase _database;
    private readonly InteractiveManager _interactive;
    private readonly GuildSettingsCache _guildSettings;

    public DoujinModule(
        IDatabase database,
        InteractiveManager interactive,
        GuildSettingsCache guildSettings)
    {
        _database = database;
        _interactive = interactive;
        _guildSettings = guildSettings;
    }

    private IDiscordContext CreateContext() => new SlashCommandContextAdapter(Context, _guildSettings);

    [SlashCommand("get", "Get information about a specific doujin")]
    public async Task GetAsync(
        [Summary("source", "The source site (nhentai, hitomi)")] string source,
        [Summary("id", "The doujin ID on the source site")] string id)
    {
        await DeferAsync();

        var doujin = await _database.GetDoujinAsync(
            GalleryUtility.ExpandContraction(source),
            id);

        if (doujin == null)
        {
            await FollowupAsync("Doujin not found.", ephemeral: true);
            return;
        }

        await _interactive.SendInteractiveAsync(
            new DoujinMessage(doujin),
            CreateContext());
    }

    [SlashCommand("get-url", "Get information about a doujin from URL")]
    public async Task GetFromUrlAsync(
        [Summary("url", "The full URL to the doujin")] string url)
    {
        var (source, id) = GalleryUtility.Parse(url);

        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(id))
        {
            await RespondAsync("Invalid URL format.", ephemeral: true);
            return;
        }

        await GetAsync(source, id);
    }

    [SlashCommand("browse", "Browse doujins from a specific source")]
    public async Task BrowseAsync(
        [Summary("source", "The source site to browse")]
        [Choice("nhentai", "nhentai")]
        [Choice("hitomi", "hitomi")]
        string source)
    {
        await DeferAsync();

        await _interactive.SendInteractiveAsync(
            new DoujinListFromSourceMessage(source),
            CreateContext());
    }

    [SlashCommand("search", "Search for doujins")]
    public async Task SearchAsync(
        [Summary("query", "Search query (tags, artists, etc.)")] string query,
        [Summary("source", "Filter by source site (optional)")]
        [Choice("nhentai", "nhentai")]
        [Choice("hitomi", "hitomi")]
        string? source = null)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            await RespondAsync("Please provide a search query.", ephemeral: true);
            return;
        }

        await DeferAsync();

        await _interactive.SendInteractiveAsync(
            new DoujinListFromQueryMessage(new DoujinSearchArgs
            {
                Query = query,
                QualityFilter = false,
                Source = GalleryUtility.ExpandContraction(source)
            }),
            CreateContext());
    }

    [SlashCommand("download", "Get download links for a doujin")]
    public async Task DownloadAsync(
        [Summary("source", "The source site (nhentai, hitomi)")] string source,
        [Summary("id", "The doujin ID on the source site")] string id)
    {
        await DeferAsync();

        var doujin = await _database.GetDoujinAsync(
            GalleryUtility.ExpandContraction(source),
            id);

        if (doujin == null)
        {
            await FollowupAsync("Doujin not found.", ephemeral: true);
            return;
        }

        await _interactive.SendInteractiveAsync(
            new DownloadMessage(doujin),
            CreateContext());
    }

    [SlashCommand("read", "Read a doujin in Discord")]
    public async Task ReadAsync(
        [Summary("source", "The source site (nhentai, hitomi)")] string source,
        [Summary("id", "The doujin ID on the source site")] string id)
    {
        await DeferAsync();

        var doujin = await _database.GetDoujinAsync(
            GalleryUtility.ExpandContraction(source),
            id);

        if (doujin == null)
        {
            await FollowupAsync("Doujin not found.", ephemeral: true);
            return;
        }

        await _interactive.SendInteractiveAsync(
            new DoujinReadMessage(doujin),
            CreateContext());
    }
}

public class SlashCommandContextAdapter : IDiscordContext
{
    private readonly SocketInteractionContext _context;
    private readonly GuildSettingsCache _guildSettings;

    public SlashCommandContextAdapter(SocketInteractionContext context, GuildSettingsCache guildSettings)
    {
        _context = context;
        _guildSettings = guildSettings;
    }

    public IDiscordClient Client => _context.Client;
    public IUserMessage? Message => null;
    public IMessageChannel Channel => _context.Channel;
    public IUser User => _context.User;
    public Guild? GuildSettings => _guildSettings[_context.Channel];
}
