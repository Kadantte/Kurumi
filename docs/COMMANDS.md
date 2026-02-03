# nhitomi Command Reference

This document lists all available slash commands for the nhitomi Discord bot.

## Doujin Commands

Commands for browsing and interacting with doujinshi content.

| Command | Description | Parameters |
|---------|-------------|------------|
| `/doujin get` | Get information about a specific doujin | `source` (nhentai/hitomi), `id` |
| `/doujin get-url` | Get information about a doujin from URL | `url` |
| `/doujin browse` | Browse doujins from a specific source | `source` (nhentai/hitomi) |
| `/doujin search` | Search for doujins | `query`, `source` (optional) |
| `/doujin download` | Get download links for a doujin | `source`, `id` |
| `/doujin read` | Read a doujin in Discord | `source`, `id` |

### Examples

```
/doujin get source:nhentai id:123456
/doujin get-url url:https://nhentai.net/g/123456/
/doujin browse source:nhentai
/doujin search query:english
/doujin download source:nhentai id:123456
/doujin read source:nhentai id:123456
```

## Collection Commands

Commands for managing personal doujin collections.

| Command | Description | Parameters |
|---------|-------------|------------|
| `/collection list` | List all your collections | - |
| `/collection view` | View a specific collection | `name` |
| `/collection add` | Add a doujin to a collection | `name`, `source`, `id` |
| `/collection remove` | Remove a doujin from a collection | `name`, `source`, `id` |
| `/collection delete` | Delete a collection | `name` |
| `/collection sort` | Change collection sort order | `name`, `sort` (Name/Artist/Group/Language/UploadTime) |

### Examples

```
/collection list
/collection view name:favorites
/collection add name:favorites source:nhentai id:123456
/collection remove name:favorites source:nhentai id:123456
/collection delete name:mylist
/collection sort name:favorites sort:Artist
```

## Settings Commands

Commands for configuring bot settings (requires Manage Server permission).

| Command | Description | Parameters |
|---------|-------------|------------|
| `/settings language` | Set the bot language for this server | `language` (en/id/ko) |

### Examples

```
/settings language language:en
```

## Feed Commands

Commands for configuring automatic feed channels (requires Manage Server permission).

| Command | Description | Parameters |
|---------|-------------|------------|
| `/feed add-tag` | Add a tag to the feed channel whitelist | `tag` |
| `/feed remove-tag` | Remove a tag from the feed whitelist | `tag` |
| `/feed mode` | Set feed channel matching mode | `mode` (any/all) |

### Examples

```
/feed add-tag tag:english
/feed remove-tag tag:japanese
/feed mode mode:any
```

## Utility Commands

General utility commands.

| Command | Description | Parameters |
|---------|-------------|------------|
| `/help` | Show help information about the bot | - |
| `/debug` | Show debug and diagnostic information | - |

## Interactive Controls

When viewing doujin information, you can use reaction buttons:

| Emoji | Action |
|-------|--------|
| ◀️ | Previous item/page |
| ▶️ | Next item/page |
| 📖 | Read the doujin |
| 💾 | Download the doujin |
| ❤️ | Add to favorites |
| 🗑️ | Delete the message |

## Permission Requirements

| Command Group | Required Permission |
|---------------|---------------------|
| `/doujin *` | None |
| `/collection *` | None (user-specific) |
| `/settings *` | Manage Server |
| `/feed *` | Manage Server |
| `/help`, `/debug` | None |

## Rate Limits

To prevent abuse, commands are rate-limited:
- **Per user**: 10 commands per 60 seconds
- **Per server**: 50 commands per 60 seconds

---

*This reference was generated from the nhitomi slash command definitions.*
