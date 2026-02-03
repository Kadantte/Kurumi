# nhitomi Glossary

This document defines key terms and concepts used throughout the nhitomi project.

## Domain Terms

| Term | Definition |
|------|------------|
| **Doujin** | Self-published Japanese work, typically manga or illustrations. The primary content type that nhitomi indexes and provides access to. |
| **Doujinshi** | Full form of "doujin" - a self-published work, typically manga. |
| **Source** | An external website that hosts doujin content (e.g., nhentai, Hitomi). nhitomi aggregates content from multiple sources. |
| **Gallery** | A collection of pages/images that make up a single doujin work. |
| **Tag** | Metadata label categorizing doujin content (e.g., artist, character, parody, language). |
| **Collection** | A user-created list of saved doujins for personal organization. |
| **Feed Channel** | A Discord channel configured to automatically receive new doujin posts matching specified criteria. |

## Technical Terms

| Term | Definition |
|------|------------|
| **Interactive Message** | A Discord embed message with reaction-based navigation and controls. |
| **Trigger** | A reaction-based action handler attached to interactive messages (e.g., page navigation, download). |
| **Guild** | Discord server. Settings are stored per-guild for customization. |
| **Slash Command** | Discord's modern command system using `/command` syntax with autocomplete and validation. |
| **Precondition** | A validation check that runs before a command executes (e.g., permission check, rate limit). |

## Source Identifiers

| Source | Website | Description |
|--------|---------|-------------|
| `nhentai` | nhentai.net | Popular English-translated doujin repository |
| `hitomi` | hitomi.la | Multi-language doujin hosting platform |

## Tag Types

| Type | Description |
|------|-------------|
| `Artist` | Creator of the doujin |
| `Group` | Circle or group that produced the work |
| `Parody` | Source material the doujin is based on |
| `Character` | Characters featured in the work |
| `Tag` | General content tags |
| `Language` | Language of the content |

## Collection Sort Options

| Sort | Description |
|------|-------------|
| `Name` | Sort by doujin title alphabetically |
| `Artist` | Sort by artist name |
| `Group` | Sort by circle/group name |
| `Language` | Sort by content language |
| `UploadTime` | Sort by when the doujin was uploaded to source |

## Feed Whitelist Modes

| Mode | Description |
|------|-------------|
| `Any` | Post doujins matching ANY of the whitelisted tags |
| `All` | Post doujins matching ALL of the whitelisted tags |

## Configuration Keys

| Key | Description |
|-----|-------------|
| `Discord:Token` | Bot authentication token from Discord Developer Portal |
| `Discord:TestGuildId` | Optional guild ID for testing slash commands during development |
| `Discord:Guild:GuildId` | Home guild ID for the bot |
| `Discord:Guild:ErrorChannelId` | Channel ID for error reporting |
| `ConnectionStrings:nhitomi` | MySQL database connection string |
| `Feed:Enabled` | Whether feed channels are enabled |

## Abbreviations

| Abbreviation | Full Form |
|--------------|-----------|
| DM | Direct Message |
| EF | Entity Framework |
| DB | Database |
| GC | Garbage Collection |
