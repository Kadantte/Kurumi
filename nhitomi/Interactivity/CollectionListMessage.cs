using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using nhitomi.Core;
using nhitomi.Discord;
using nhitomi.Interactivity.Triggers;

namespace nhitomi.Interactivity
{
    public class CollectionListMessage : ListMessage<CollectionListMessage.View, Collection>
    {
        readonly ulong _userId;

        public CollectionListMessage(ulong userId)
        {
            _userId = userId;
        }

        protected override IEnumerable<IReactionTrigger> CreateTriggers()
        {
            yield return new ListTrigger(MoveDirection.Left);
            yield return new ListTrigger(MoveDirection.Right);
            yield return new DeleteTrigger();
        }

        public class View : ListViewBase
        {
            new CollectionListMessage Message => (CollectionListMessage) base.Message;

            readonly IDatabase _db;

            public View(IDatabase db)
            {
                _db = db;
            }

            protected override bool ShowLoadingIndication => false;

            protected override Task<Collection[]> GetValuesAsync(int offset,
                                                                 CancellationToken cancellationToken = default) =>
                offset == 0
                    ? _db.GetCollectionsAsync(Message._userId, cancellationToken)
                    : Task.FromResult(new Collection[0]);

            protected override Embed CreateEmbed(Collection collection)
            {
                var l = Context.GetLocalization()["collectionMessage"];

                var embed = new EmbedBuilder
                {
                    Title = $"**{Context.User.Username}**: {collection.Name}",
                    Color = Color.Teal,

                    Fields = new List<EmbedFieldBuilder>
                    {
                        new EmbedFieldBuilder
                        {
                            Name  = l["sort"],
                            Value = l["sortValue"][collection.Sort.ToString()]
                        },
                        new EmbedFieldBuilder
                        {
                            Name  = l["contents"],
                            Value = l["contentsValue", new { collection }]
                        }
                    }
                };

                if (collection.Doujins.Count == 0)
                    embed.Description = l["emptyCollection"];
                else
                    embed.ThumbnailUrl =
                        $"https://nhitomi.chiya.dev/api/v1/images/{collection.Doujins.First().DoujinId}/-1";

                return embed.Build();
            }

            protected override Embed CreateEmptyEmbed()
            {
                var l = Context.GetLocalization()["collectionMessage"]["emptyList"];

                return new EmbedBuilder
                {
                    Title       = l["title"],
                    Color       = Color.Teal,
                    Description = l["text", new { context = Context }]
                }.Build();
            }

            protected override string ListBeginningMessage => "collectionMessage.collectionListBeginning";
            protected override string ListEndMessage => "collectionMessage.collectionListEnd";
        }
    }
}