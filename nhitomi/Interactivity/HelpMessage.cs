using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord;
using Microsoft.Extensions.Options;
using nhitomi.Discord;
using nhitomi.Globalization;
using nhitomi.Interactivity.Triggers;

namespace nhitomi.Interactivity
{
    public enum HelpMessageSection
    {
        Doujins,
        Collections,
        Options,
        Other
    }

    public class HelpMessage : ListMessage<HelpMessage.View, HelpMessageSection>
    {
        protected override IEnumerable<IReactionTrigger> CreateTriggers()
        {
            yield return new ListTrigger(MoveDirection.Left);
            yield return new ListTrigger(MoveDirection.Right);
            yield return new DeleteTrigger();
        }

        public class View : SynchronousListViewBase
        {
            readonly AppSettings _settings;

            public View(IOptions<AppSettings> options)
            {
                _settings = options.Value;
            }

            protected override HelpMessageSection[] GetValues(int offset) => Enum.GetValues(typeof(HelpMessageSection))
                                                                                 .Cast<HelpMessageSection>()
                                                                                 .Skip(offset)
                                                                                 .ToArray();

            protected override Embed CreateEmbed(HelpMessageSection value)
            {
                var l = Context.GetLocalization()["helpMessage"];

                var embed = new EmbedBuilder
                {
                    Title        = $"**nhitomi**: {l["title"]}",
                    Color        = Color.Purple,
                    ThumbnailUrl = "https://github.com/chiyadev/nhitomi/raw/master/nhitomi.png",
                    Footer = new EmbedFooterBuilder
                    {
                        Text = $"v{VersionHelper.Version.ToString(2)} {VersionHelper.Codename} — {l["footer"]}"
                    }
                };

                switch (value)
                {
                    case HelpMessageSection.Doujins:

                        embed.Description =
                            $"nhitomi — {l["about"]}\n\n" +
                            $"{l["invite", new { botInvite = _settings.Discord.BotInvite, guildInvite = _settings.Discord.Guild.GuildInvite }]}";

                        DoujinsSection(embed, l);
                        SourcesSection(embed, l);
                        break;

                    case HelpMessageSection.Collections:
                        CollectionsSection(embed, l);
                        break;

                    case HelpMessageSection.Options:
                        OptionsSection(embed, l);
                        LanguagesSection(embed, l);
                        break;

                    case HelpMessageSection.Other:
                        // only add translators if not English
                        if (l.Localization != Localization.Default)
                            TranslationsSection(embed, l);

                        OpenSourceSection(embed, l);
                        break;
                }

                return embed.Build();
            }

            void DoujinsSection(EmbedBuilder embed,
                                LocalizationAccess l)
            {
                l = l["doujins"];

                embed.AddField(l["title"],
                               $@"
- `/doujin get` — {l["get"]}
- `/doujin get-url` — {l["from"]}
- `/doujin read` — {l["read"]}
- `/doujin download` — {l["download"]}
- `/doujin search` — {l["search"]}
- `/doujin browse` — Browse all doujins from a source
".Trim());
            }

            static void SourcesSection(EmbedBuilder embed,
                                       LocalizationAccess l)
            {
                l = l["doujins"]["sources"];

                embed.AddField(l["title"],
                               @"
- nhentai — `https://nhentai.net/`
- Hitomi — `https://hitomi.la/`
".Trim());
            }

            static void CollectionsSection(EmbedBuilder embed,
                                    LocalizationAccess l)
            {
                l = l["collections"];

                embed.AddField(l["title"],
                               $@"
- `/collection list` — {l["list"]}
- `/collection view` — {l["view"]}
- `/collection add` — {l["add"]}
- `/collection remove` — {l["remove"]}
- `/collection sort` — {l["sort"]}
- `/collection delete` — {l["delete"]}
".Trim());
            }

            static void OptionsSection(EmbedBuilder embed,
                                LocalizationAccess l)
            {
                l = l["options"];

                embed.AddField(l["title"],
                               $@"
- `/settings language` — {l["language"]}
- `/feed add-tag` — {l["feed.add"]}
- `/feed remove-tag` — {l["feed.remove"]}
- `/feed mode` — {l["feed.mode"]}
".Trim());
            }

            static void LanguagesSection(EmbedBuilder embed,
                                         LocalizationAccess l)
            {
                l = l["options"]["languages"];

                var content = new StringBuilder();

                foreach (var localization in Localization.GetAllLocalizations())
                {
                    var culture = localization.Culture;

                    content.AppendLine($"- `{culture.Name}` — {culture.EnglishName} | {culture.NativeName}");
                }

                embed.AddField(l["title"], content.ToString());
            }

            static void TranslationsSection(EmbedBuilder embed,
                                            LocalizationAccess l)
            {
                l = l["translations"];

                embed.AddField(
                    l["title"],
                    l["text", new { translators = l.Localization["meta"]["translators"] }]);
            }

            static void OpenSourceSection(EmbedBuilder embed,
                                          LocalizationAccess l)
            {
                l = l["openSource"];

                embed.AddField(l["title"],
                               $@"
{l["license"]}
[GitHub](https://github.com/chiyadev/nhitomi)
".Trim());
            }

            protected override Embed CreateEmptyEmbed() => throw new NotSupportedException();

            protected override string ListBeginningMessage => "helpMessage.listBeginning";
            protected override string ListEndMessage => "helpMessage.listEnd";
        }
    }
}