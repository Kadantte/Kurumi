using Discord;
using Discord.Interactions;
using nhitomi.Core;
using nhitomi.Discord;
using nhitomi.Interactivity;

namespace nhitomi.Modules;

[Group("collection", "Manage your doujin collections")]
[RateLimit]
public class CollectionModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IDatabase _database;
    private readonly InteractiveManager _interactive;
    private readonly GuildSettingsCache _guildSettings;

    public CollectionModule(
        IDatabase database,
        InteractiveManager interactive,
        GuildSettingsCache guildSettings)
    {
        _database = database;
        _interactive = interactive;
        _guildSettings = guildSettings;
    }

    private IDiscordContext CreateContext() => new SlashCommandContextAdapter(Context, _guildSettings);

    private static string FixCollectionName(string name)
    {
        return name.ToLowerInvariant() switch
        {
            "favs" => "favorites",
            _ => name
        };
    }

    [SlashCommand("list", "List all your collections")]
    public async Task ListAsync()
    {
        await DeferAsync();
        await _interactive.SendInteractiveAsync(
            new CollectionListMessage(Context.User.Id),
            CreateContext());
    }

    [SlashCommand("view", "View a specific collection")]
    public async Task ViewAsync(
        [Summary("name", "Name of the collection to view")] string name)
    {
        name = FixCollectionName(name);

        await DeferAsync();

        var collection = await _database.GetCollectionAsync(Context.User.Id, name);

        if (collection == null)
        {
            await FollowupAsync($"Collection '{name}' not found.", ephemeral: true);
            return;
        }

        await _interactive.SendInteractiveAsync(
            new CollectionMessage(Context.User.Id, name),
            CreateContext());
    }

    [SlashCommand("add", "Add a doujin to a collection")]
    public async Task AddAsync(
        [Summary("name", "Name of the collection (will be created if it doesn't exist)")] string name,
        [Summary("source", "The source site (nhentai, hitomi)")] string source,
        [Summary("id", "The doujin ID on the source site")] string id)
    {
        name = FixCollectionName(name);

        await DeferAsync();

        Doujin? doujin;
        Collection? collection;

        do
        {
            collection = await _database.GetCollectionAsync(Context.User.Id, name);

            if (collection == null)
            {
                collection = new Collection
                {
                    Name = name,
                    OwnerId = Context.User.Id,
                    Doujins = new List<CollectionRef>()
                };

                _database.Add(collection);
            }

            doujin = await _database.GetDoujinAsync(
                GalleryUtility.ExpandContraction(source),
                id);

            if (doujin == null)
            {
                await FollowupAsync("Doujin not found.", ephemeral: true);
                return;
            }

            if (collection.Doujins.Any(x => x.DoujinId == doujin.Id))
            {
                await FollowupAsync($"'{doujin.PrettyName}' is already in collection '{collection.Name}'.", ephemeral: true);
                return;
            }

            collection.Doujins.Add(new CollectionRef
            {
                DoujinId = doujin.Id
            });
        }
        while (!await _database.SaveAsync());

        await FollowupAsync($"Added '{doujin.PrettyName}' to collection '{collection.Name}'.");
    }

    [SlashCommand("remove", "Remove a doujin from a collection")]
    public async Task RemoveAsync(
        [Summary("name", "Name of the collection")] string name,
        [Summary("source", "The source site (nhentai, hitomi)")] string source,
        [Summary("id", "The doujin ID on the source site")] string id)
    {
        name = FixCollectionName(name);

        await DeferAsync();

        Doujin? doujin;
        Collection? collection;

        do
        {
            collection = await _database.GetCollectionAsync(Context.User.Id, name);

            if (collection == null)
            {
                await FollowupAsync($"Collection '{name}' not found.", ephemeral: true);
                return;
            }

            doujin = await _database.GetDoujinAsync(
                GalleryUtility.ExpandContraction(source),
                id);

            if (doujin == null)
            {
                await FollowupAsync("Doujin not found.", ephemeral: true);
                return;
            }

            var item = collection.Doujins.FirstOrDefault(x => x.DoujinId == doujin.Id);

            if (item == null)
            {
                await FollowupAsync($"'{doujin.PrettyName}' is not in collection '{collection.Name}'.", ephemeral: true);
                return;
            }

            collection.Doujins.Remove(item);
        }
        while (!await _database.SaveAsync());

        await FollowupAsync($"Removed '{doujin.PrettyName}' from collection '{collection.Name}'.");
    }

    [SlashCommand("delete", "Delete a collection")]
    public async Task DeleteAsync(
        [Summary("name", "Name of the collection to delete")] string name)
    {
        name = FixCollectionName(name);

        await DeferAsync();

        Collection? collection;

        do
        {
            collection = await _database.GetCollectionAsync(Context.User.Id, name);

            if (collection == null)
            {
                await FollowupAsync($"Collection '{name}' not found.", ephemeral: true);
                return;
            }

            _database.Remove(collection);
        }
        while (!await _database.SaveAsync());

        await FollowupAsync($"Deleted collection '{collection.Name}'.");
    }

    [SlashCommand("sort", "Change the sort order of a collection")]
    public async Task SortAsync(
        [Summary("name", "Name of the collection")] string name,
        [Summary("sort", "How to sort the collection")]
        [Choice("Name", "name")]
        [Choice("Artist", "artist")]
        [Choice("Group", "group")]
        [Choice("Language", "language")]
        [Choice("Upload Time", "uploadtime")]
        string sort)
    {
        name = FixCollectionName(name);

        await DeferAsync();

        if (!Enum.TryParse<CollectionSort>(sort, true, out var sortEnum))
        {
            await FollowupAsync("Invalid sort option.", ephemeral: true);
            return;
        }

        Collection? collection;

        do
        {
            collection = await _database.GetCollectionAsync(Context.User.Id, name);

            if (collection == null)
            {
                await FollowupAsync($"Collection '{name}' not found.", ephemeral: true);
                return;
            }

            collection.Sort = sortEnum;
        }
        while (!await _database.SaveAsync());

        await FollowupAsync($"Collection '{collection.Name}' is now sorted by {sortEnum}.");
    }
}
