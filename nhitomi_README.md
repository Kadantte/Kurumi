<!--
 Copyright (c) 2018-2019 chiya.dev

 This software is released under the MIT License.
 https://opensource.org/licenses/MIT
-->

# nhitomi ![Build status](https://ci.appveyor.com/api/projects/status/vtdjarua2c9i0k5t?svg=true)

**Version 3.4 (Heresta)** — A Discord bot for searching and downloading doujinshi by [chiya.dev](https://chiya.dev).

![nhitomi](nhitomi.png)

Join our [Discord server](https://discord.gg/JFNga7q) or [invite nhitomi](https://discordapp.com/oauth2/authorize?client_id=515386276543725568&scope=bot&permissions=347200) to your server.

## Commands

All commands use Discord slash commands. Type `/` in Discord to see available commands.

### Doujinshi

- `/doujin get source id` — Displays doujin information from a source by its ID.
- `/doujin get-url url` — Displays doujin information from a URL.
- `/doujin browse source` — Browse all doujins from a source.
- `/doujin search query` — Searches for doujins by tags and title.
- `/doujin download source id` — Sends a download link for a doujin.
- `/doujin read source id` — Read a doujin in Discord.

### Collection management

- `/collection list` — Lists all collections belonging to you.
- `/collection view name` — Displays doujins belonging to a collection.
- `/collection add name source id` — Adds a doujin to a collection.
- `/collection remove name source id` — Removes a doujin from a collection.
- `/collection sort name sort` — Sorts doujins in a collection.
- `/collection delete name` — Deletes a collection.

### Feed channels (Admin)

- `/feed add-tag tag` — Adds a tag to the feed channel whitelist.
- `/feed remove-tag tag` — Removes a tag from the feed channel whitelist.
- `/feed mode mode` — Sets the feed channel matching mode.

### Settings (Admin)

- `/settings language language` — Sets the bot language for the server.

### Help

- `/help` — Shows help information.
- `/debug` — Shows debug and diagnostic information.

### Sources

- nhentai — `https://nhentai.net/`
- hitomi — `https://hitomi.la/`

## Running nhitomi

### Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or higher.
- For development: a C# IDE with intellisense and syntax highlighting, such as [Visual Studio Code](https://code.visualstudio.com/) or [JetBrains Rider](https://www.jetbrains.com/rider/).

### Building

Create a file named `appsettings.Development.json` alongside `appsettings.json`. Then paste the following code, replacing the token string with your own:

```json
{
  "Discord": {
    "Token": "YOUR_BOT_TOKEN_HERE",
    "TestGuildId": 123456789012345678
  },
  "ConnectionStrings": {
    "nhitomi": "Server=localhost;Database=nhitomi;User=root;Password=password;"
  }
}
```

Then run the following commands:

1. `dotnet restore` — resolves NuGet dependencies.
2. `dotnet build` — builds the bot.
3. `dotnet run --project nhitomi` — runs the bot.

### Docker

```bash
docker build -t nhitomi .
docker run -e Discord__Token=YOUR_TOKEN nhitomi
```

## License

Copyright (c) 2018-2019 chiya.dev

This project is licensed under the [MIT license](https://opensource.org/licenses/MIT). Please see the [LICENSE](LICENSE) file for more information.
