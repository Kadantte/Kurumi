# CLAUDE CODE ANALYSIS REPORT

**Project:** nhitomi (Kurumi)
**Analysis Date:** 2026-02-03
**Analyzer:** Claude Opus 4.5

---

## TABLE OF CONTENTS

1. [Executive Summary](#1-executive-summary)
2. [Project Overview](#2-project-overview)
3. [Technology Stack](#3-technology-stack)
4. [Project Structure](#4-project-structure)
5. [Architecture & Design Patterns](#5-architecture--design-patterns)
6. [Database Schema](#6-database-schema)
7. [Core Components](#7-core-components)
8. [Command System](#8-command-system)
9. [Doujin Source Clients](#9-doujin-source-clients)
10. [Interactive Message System](#10-interactive-message-system)
11. [Configuration System](#11-configuration-system)
12. [Localization & Internationalization](#12-localization--internationalization)
13. [External Integrations](#13-external-integrations)
14. [Data Flow Analysis](#14-data-flow-analysis)
15. [Error Handling & Monitoring](#15-error-handling--monitoring)
16. [Deployment & CI/CD](#16-deployment--cicd)
17. [File Inventory](#17-file-inventory)
18. [Dependencies](#18-dependencies)
19. [Security Considerations](#19-security-considerations)
20. [Recommendations](#20-recommendations)

---

## 1. EXECUTIVE SUMMARY

**nhitomi** is a feature-rich Discord bot application written in C# targeting .NET Core 2.2. The bot is designed for searching, browsing, and managing doujinshi (adult manga) from multiple online sources including nhentai.net and hitomi.la.

### Key Highlights

| Aspect | Details |
|--------|---------|
| **Language** | C# (.NET Core 2.2) |
| **Type** | Discord Bot Application |
| **License** | MIT License (2018-2019 chiya.dev) |
| **Version** | 3.4 (Heresta) |
| **Database** | MySQL (Production) / SQLite (Development) |
| **Primary Framework** | Discord.Net 2.1.1 |

### Core Capabilities

- Multi-source doujinshi search and browsing
- User collection management
- Interactive paginated messages with reaction controls
- Automated feed channels for doujin distribution
- Multi-language support (English, Indonesian, Korean)
- Download link generation

---

## 2. PROJECT OVERVIEW

### Purpose

nhitomi provides Discord users with commands to:
- Search and browse doujinshi from multiple sources
- View detailed information about individual works
- Manage personal collections of favorites
- Configure automatic feed channels in servers
- Generate download links for offline viewing
- Read/view pages directly in Discord

### Repository Information

| Field | Value |
|-------|-------|
| **Original Repository** | https://github.com/chiyadev/nhitomi |
| **Copyright** | 2018-2019 chiya.dev |
| **License** | MIT License |

### Version History

The project has undergone multiple iterations with codenames:
- Current: Version 3.4 "Heresta"

---

## 3. TECHNOLOGY STACK

### Core Framework

```
.NET Core 2.2 (netcoreapp2.2)
.NET Standard 2.0 (Core library)
```

### Primary Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| Discord.Net | 2.1.1 | Discord API client library |
| Microsoft.AspNetCore | 2.2.0 | Web framework & hosting |
| Microsoft.EntityFrameworkCore | 2.2.4 | ORM / Database access |
| Pomelo.EntityFrameworkCore.MySql | 2.2.0 | MySQL provider |
| Microsoft.EntityFrameworkCore.Sqlite | 2.2.4 | SQLite provider (dev) |
| HtmlAgilityPack | 1.11.7 | HTML parsing for web scraping |
| Newtonsoft.Json | 12.0.2 | JSON serialization |
| SmartFormat.NET | 2.4.2 | String formatting/localization |
| System.Interactive.Async | 3.2.0 | Async LINQ extensions |

### Database

| Environment | Database | Provider |
|-------------|----------|----------|
| Production | MySQL | Pomelo.EntityFrameworkCore.MySql |
| Development | SQLite | Microsoft.EntityFrameworkCore.Sqlite |

### Build & Deployment

| Tool | Purpose |
|------|---------|
| AppVeyor | CI/CD pipeline |
| Docker | Containerization |
| Renovate | Automated dependency updates |

---

## 4. PROJECT STRUCTURE

```
Kurumi/
├── nhitomi/                          # Main application project
│   ├── Program.cs                    # Entry point
│   ├── Startup.cs                    # DI configuration
│   ├── AppSettings.cs                # Configuration models
│   ├── GalleryUtility.cs             # URL parsing utilities
│   ├── VersionHelper.cs              # Version information
│   ├── AsyncEnumerableBrowser.cs     # Async enumeration helpers
│   ├── DependencyUtility.cs          # DI utilities
│   ├── ForcedGarbageCollector.cs     # Memory management
│   │
│   ├── Discord/                      # Discord bot core
│   │   ├── DiscordService.cs         # Discord client wrapper
│   │   ├── CommandExecutor.cs        # Command parsing
│   │   ├── MessageHandlerService.cs  # Message events
│   │   ├── ReactionHandlerService.cs # Reaction events
│   │   ├── GalleryUrlDetector.cs     # URL detection
│   │   ├── FeedChannelUpdateService.cs
│   │   ├── GuildSettingsSyncService.cs
│   │   ├── GuildWelcomeMessageService.cs
│   │   ├── DiscordErrorReporter.cs
│   │   ├── LogHandlerService.cs
│   │   ├── StatusUpdateService.cs
│   │   │
│   │   └── Parsing/                  # Command parsing
│   │       ├── CommandAttribute.cs
│   │       ├── ModuleAttribute.cs
│   │       ├── OptionAttribute.cs
│   │       ├── BindingAttribute.cs
│   │       └── CommandInfo.cs
│   │
│   ├── Modules/                      # Command modules
│   │   ├── DoujinModule.cs           # Doujin commands
│   │   ├── CollectionModule.cs       # Collection commands
│   │   ├── FeedModule.cs             # Feed commands
│   │   ├── HelpModule.cs             # Help commands
│   │   └── OptionModule.cs           # Options commands
│   │
│   ├── Interactivity/                # Interactive messages
│   │   ├── InteractiveManager.cs
│   │   ├── InteractiveMessage.cs
│   │   ├── EmbedMessage.cs
│   │   ├── ListMessage.cs
│   │   ├── DoujinMessage.cs
│   │   ├── DoujinListMessage.cs
│   │   ├── DoujinListFromQueryMessage.cs
│   │   ├── DoujinListFromSourceMessage.cs
│   │   ├── CollectionMessage.cs
│   │   ├── CollectionListMessage.cs
│   │   ├── DownloadMessage.cs
│   │   ├── DoujinReadMessage.cs
│   │   ├── CommandHelpMessage.cs
│   │   ├── ErrorMessage.cs
│   │   ├── HelpMessage.cs
│   │   │
│   │   └── Triggers/                 # Reaction triggers
│   │       ├── ReactionTrigger.cs
│   │       ├── DeleteTrigger.cs
│   │       ├── DownloadTrigger.cs
│   │       ├── FavoriteTrigger.cs
│   │       ├── ReadTrigger.cs
│   │       ├── ListTrigger.cs
│   │       └── ListJumpTrigger.cs
│   │
│   ├── Globalization/                # Localization
│   │   ├── Localization.cs
│   │   ├── LocalizationDictionary.cs
│   │   └── Languages/
│   │       ├── en.json               # English
│   │       ├── id.json               # Indonesian
│   │       └── ko.json               # Korean
│   │
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   └── nhitomi.csproj
│
├── nhitomi.Core/                     # Core library
│   ├── Doujin.cs                     # Doujin entity
│   ├── DoujinInfo.cs                 # DTO
│   ├── Collection.cs                 # Collection entity
│   ├── Tag.cs                        # Tag entity
│   ├── Guild.cs                      # Guild entity
│   ├── FeedChannel.cs                # Feed channel entity
│   ├── CollectionSort.cs
│   ├── Extensions.cs
│   ├── HashUtility.cs
│   ├── AtomicCounter.cs
│   ├── IDoujinClient.cs
│   ├── IHttpClient.cs
│   ├── HttpClientWrapper.cs
│   ├── IgnoredAttribute.cs
│   ├── nhitomiDbContext.cs           # EF Core context
│   ├── nhitomiSerializerSettings.cs
│   │
│   ├── Clients/                      # Source clients
│   │   ├── IDoujinClient.cs
│   │   ├── ClientTester.cs
│   │   ├── nhentai/
│   │   │   ├── nhentaiClient.cs
│   │   │   ├── nhentai82843.cs
│   │   │   ├── nhentai250000.cs
│   │   │   └── nhentai273650.cs
│   │   └── Hitomi/
│   │       ├── HitomiClient.cs
│   │       ├── Hitomi1422964.cs
│   │       ├── Hitomi1425272.cs
│   │       └── Hitomi1425386.cs
│   │
│   ├── Models/
│   │   ├── Doujin.cs
│   │   ├── DoujinMeta.cs
│   │   ├── DoujinSource.cs
│   │   ├── DoujinCollection.cs
│   │   └── ModelBase.cs
│   │
│   ├── Migrations/
│   │   ├── 20190610042252_MergeMigrations.cs
│   │   └── nhitomiDbContextModelSnapshot.cs
│   │
│   └── nhitomi.Core.csproj
│
├── nhitomi.Core.UnitTests/           # Unit tests
│   ├── TestUtils.cs
│   └── Clients/
│       └── ClientTests.cs
│
├── appveyor.yml                      # CI/CD config
├── Dockerfile                        # Docker config
├── renovate.json                     # Dependency automation
├── nhitomi.sln                       # Solution file
├── README.md
├── nhitomi_README.md
└── LICENSE
```

---

## 5. ARCHITECTURE & DESIGN PATTERNS

### Architectural Style

The application follows a **layered architecture** with clear separation of concerns:

```
┌─────────────────────────────────────────────────────┐
│                   DISCORD LAYER                      │
│  (Commands, Messages, Reactions, Events)            │
├─────────────────────────────────────────────────────┤
│                   SERVICE LAYER                      │
│  (Handlers, Managers, Executors)                    │
├─────────────────────────────────────────────────────┤
│                   BUSINESS LAYER                     │
│  (Modules, Interactive System, Clients)             │
├─────────────────────────────────────────────────────┤
│                    DATA LAYER                        │
│  (DbContext, Entities, Repositories)                │
└─────────────────────────────────────────────────────┘
```

### Design Patterns Used

| Pattern | Implementation | Location |
|---------|----------------|----------|
| **Dependency Injection** | Microsoft.Extensions.DependencyInjection | `Startup.cs` |
| **Repository Pattern** | IDatabase abstraction | `nhitomiDbContext.cs` |
| **Service Pattern** | IHostedService | All background services |
| **Factory Pattern** | DependencyUtility | `DependencyUtility.cs` |
| **Decorator Pattern** | DiscordContextWrapper | Discord context wrapping |
| **Observer Pattern** | Discord event handlers | Message/Reaction handlers |
| **State Pattern** | InteractiveMessage states | Interactivity system |
| **Strategy Pattern** | IDoujinClient implementations | Source clients |
| **Command Pattern** | Module/Command attributes | Command system |

### Dependency Injection Container

Services are registered in `Startup.ConfigureServices()`:

```csharp
// Core services
services.AddDbContext<nhitomiDbContext>();
services.AddSingleton<IDiscordClient>();
services.AddSingleton<InteractiveManager>();

// Hosted services (background tasks)
services.AddHostedService<MessageHandlerService>();
services.AddHostedService<ReactionHandlerService>();
services.AddHostedService<StatusUpdateService>();
services.AddHostedService<FeedChannelUpdateService>();
services.AddHostedService<GuildSettingsSyncService>();
```

---

## 6. DATABASE SCHEMA

### Entity Relationship Diagram

```
┌─────────────────┐     ┌─────────────────┐
│     GUILD       │     │   FEEDCHANNEL   │
├─────────────────┤     ├─────────────────┤
│ Id (PK)         │◄────┤ GuildId (FK)    │
│ Language        │     │ Id (PK)         │
│ SearchFilter    │     │ LastDoujinId    │
└─────────────────┘     │ WhitelistType   │
                        └────────┬────────┘
                                 │
                        ┌────────┴────────┐
                        │ FEEDCHANNELTAG  │
                        │ (Join Table)    │
                        └────────┬────────┘
                                 │
┌─────────────────┐     ┌────────▼────────┐
│     DOUJIN      │◄────┤      TAG        │
├─────────────────┤     ├─────────────────┤
│ Id (PK)         │     │ Id (PK)         │
│ AccessId        │     │ AccessId        │
│ PrettyName      │     │ Type            │
│ OriginalName    │     │ Value           │
│ UploadTime      │     └─────────────────┘
│ ProcessTime     │
│ Source          │     ┌─────────────────┐
│ SourceId        │     │   COLLECTION    │
│ PageCount       │     ├─────────────────┤
│ Data            │◄────┤ Id (PK)         │
│ TagsDenormalized│     │ Name            │
└─────────────────┘     │ OwnerId         │
                        │ Sort            │
                        │ SortDescending  │
                        └─────────────────┘
```

### Entity Definitions

#### Doujin
Primary entity representing a doujinshi work.

| Column | Type | Description |
|--------|------|-------------|
| Id | int | Primary key |
| AccessId | Guid | Unique public identifier |
| PrettyName | string | English/romanized name |
| OriginalName | string | Original language name |
| UploadTime | DateTime | Upload timestamp |
| ProcessTime | DateTime | Processing timestamp |
| Source | string | Source identifier (nhentai, hitomi) |
| SourceId | string | Source-specific ID |
| PageCount | int | Number of pages |
| Data | string | Client-specific JSON data |
| TagsDenormalized | string | Flattened tags for search |

#### Tag
Metadata tags for doujins.

| Column | Type | Description |
|--------|------|-------------|
| Id | int | Primary key |
| AccessId | Guid | Unique public identifier |
| Type | TagType | Enum: Artist, Group, Scanlator, Language, Parody, Character, Category, Tag |
| Value | string | Tag value/name |

#### Collection
User-created collections.

| Column | Type | Description |
|--------|------|-------------|
| Id | int | Primary key |
| Name | string | Collection name |
| OwnerId | ulong | Discord user ID |
| Sort | CollectionSort | Sort criteria |
| SortDescending | bool | Sort direction |

#### Guild
Discord server settings.

| Column | Type | Description |
|--------|------|-------------|
| Id | ulong | Discord guild ID (primary key) |
| Language | string | Preferred language code |
| SearchQualityFilter | bool | Enable quality filtering |

#### FeedChannel
Automated feed channel configuration.

| Column | Type | Description |
|--------|------|-------------|
| Id | ulong | Discord channel ID (primary key) |
| GuildId | ulong | Parent guild ID |
| LastDoujinId | int? | Last sent doujin (tracking) |
| WhitelistType | WhitelistType | Any/All tag matching |

### Join Tables

| Table | Relationship |
|-------|--------------|
| TagRef | Doujin ↔ Tag (many-to-many) |
| CollectionRef | Collection ↔ Doujin (many-to-many) |
| FeedChannelTag | FeedChannel ↔ Tag (many-to-many) |

---

## 7. CORE COMPONENTS

### 7.1 Entry Point (Program.cs)

The application entry point creates and runs the host:

```csharp
public static async Task Main(string[] args) =>
    await CreateHostBuilder(args).Build().RunAsync();

public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(builder =>
            builder.UseStartup<Startup>());
```

### 7.2 Startup Configuration (Startup.cs)

Handles:
- Configuration loading (JSON files, environment variables, secrets)
- Service registration in DI container
- Database context setup
- Discord client initialization
- Background service registration

### 7.3 Discord Service (DiscordService.cs)

Wraps the Discord.Net client providing:
- Connection management
- Event subscription
- Client configuration
- Socket caching

### 7.4 Interactive Manager (InteractiveManager.cs)

Manages interactive messages:
- Tracks active interactive messages
- Routes reaction events to handlers
- Handles message expiration (1 hour timeout)
- Manages concurrent user interactions

### 7.5 Database Context (nhitomiDbContext.cs)

Entity Framework Core context:
- DbSet definitions for all entities
- Relationship configuration
- Index definitions
- Migration support

---

## 8. COMMAND SYSTEM

### Command Framework

The bot uses a custom reflection-based command framework with attributes:

```csharp
[Module("module")]
public class ExampleModule
{
    [Command("command", Alias = "c")]
    [Binding("[arg1] [arg2]")]
    public async Task<IInteractiveMessage> CommandAsync(
        string arg1,
        [Option] string arg2 = "default")
    {
        // Implementation
    }
}
```

### Command Parsing Pipeline

```
1. Message Received
       │
       ▼
2. Check Prefix (default: "n!")
       │
       ▼
3. Reflect Over Modules
       │
       ▼
4. Find Matching Command
       │
       ▼
5. Parse Arguments (Binding Pattern)
       │
       ▼
6. Resolve Dependencies
       │
       ▼
7. Invoke Command Method
       │
       ▼
8. Return Result to Discord
```

### Available Commands

#### DoujinModule

| Command | Alias | Description | Usage |
|---------|-------|-------------|-------|
| `get` | `g` | Get doujin info | `n!get nhentai 12345` |
| `from` | `f` | Browse source | `n!from nhentai` |
| `search` | `s` | Search doujins | `n!search "query"` |
| `download` | `dl` | Get download link | `n!download nhentai 12345` |
| `read` | `r` | Open reader | `n!read nhentai 12345` |

#### CollectionModule

| Command | Alias | Description | Usage |
|---------|-------|-------------|-------|
| `collections` | `c` | List collections | `n!collections` |
| `collection` | - | View/manage collection | `n!collection favs` |
| `collection add` | - | Add to collection | `n!collection favs add nhentai 12345` |
| `collection remove` | - | Remove from collection | `n!collection favs remove nhentai 12345` |
| `collection sort` | - | Sort collection | `n!collection favs sort name` |
| `collection delete` | - | Delete collection | `n!collection favs delete` |

#### HelpModule

| Command | Alias | Description |
|---------|-------|-------------|
| `help` | `h` | Show help |
| `debug` | - | Show debug stats |

#### OptionModule

| Command | Description |
|---------|-------------|
| `language` | Set language preference |
| `quality` | Toggle quality filter |

#### FeedModule

| Command | Description |
|---------|-------------|
| `feed` | Configure feed channels |

---

## 9. DOUJIN SOURCE CLIENTS

### Client Interface

```csharp
public interface IDoujinClient
{
    string Name { get; }
    string Url { get; }
    string IconUrl { get; }
    GalleryRegex GalleryRegex { get; }

    Task<DoujinInfo> GetAsync(string id);
    Task<IEnumerable<string>> EnumerateAsync(string startId = null);
    IEnumerable<string> PopulatePages(Doujin doujin);
    void InitializeImageRequest(Doujin doujin, HttpRequestMessage request);
}
```

### Implemented Clients

#### nhentaiClient

| Property | Value |
|----------|-------|
| **Source** | nhentai.net |
| **API Type** | JSON REST API |
| **Base URL** | `https://nhentai.net/api/galleries/` |
| **Features** | Pagination, search, metadata |

Implementation details:
- Uses JSON API for efficient data retrieval
- Handles rate limiting
- Generates CDN URLs for images

#### HitomiClient

| Property | Value |
|----------|-------|
| **Source** | hitomi.la |
| **API Type** | HTML scraping + custom format |
| **Base URL** | `https://hitomi.la/` |
| **Features** | NOZOMI index, CDN delivery |

Implementation details:
- HTML parsing with HtmlAgilityPack
- Uses NOZOMI binary index format
- Custom CDN URL generation logic
- JavaScript-style URL computation

### URL Detection (GalleryUtility)

The bot can detect doujin URLs in messages:

```
Supported Formats:
- Full URLs: nhentai.net/g/12345, hitomi.la/galleries/12345.html
- Short forms: nh/12345, hi/12345
- Direct IDs: 12345 (context-dependent)
```

### Test Cases

Each client has test cases for validation:

**nhentai:**
- ID 82843
- ID 250000
- ID 273650

**Hitomi:**
- ID 1422964
- ID 1425272
- ID 1425386

---

## 10. INTERACTIVE MESSAGE SYSTEM

### Overview

The interactive system enables rich, reactive Discord embeds with pagination and user interactions.

### Components

```
InteractiveManager
       │
       ├── InteractiveMessage (base)
       │         │
       │         ├── DoujinMessage
       │         ├── DoujinListMessage
       │         │        ├── DoujinListFromQueryMessage
       │         │        └── DoujinListFromSourceMessage
       │         ├── CollectionMessage
       │         ├── CollectionListMessage
       │         ├── DownloadMessage
       │         ├── DoujinReadMessage
       │         ├── HelpMessage
       │         ├── CommandHelpMessage
       │         └── ErrorMessage
       │
       └── ReactionTrigger (base)
                 │
                 ├── ListTrigger (◄ ►)
                 ├── ListJumpTrigger
                 ├── DeleteTrigger
                 ├── DownloadTrigger
                 ├── FavoriteTrigger
                 └── ReadTrigger
```

### Message Lifecycle

```
1. Command Executed
       │
       ▼
2. Create InteractiveMessage
       │
       ▼
3. Send to Discord
       │
       ▼
4. Add Reaction Triggers
       │
       ▼
5. Register with InteractiveManager
       │
       ▼
6. Wait for User Reactions
       │
       ▼
7. Handle Trigger Events
       │
       ▼
8. Update Message Content
       │
       ▼
9. Auto-expire after 1 hour
```

### Reaction Triggers

| Trigger | Emoji | Function |
|---------|-------|----------|
| ListTrigger | ◄ ► | Navigate pages |
| ListJumpTrigger | 🔢 | Jump to page |
| DeleteTrigger | ❌ | Delete message |
| DownloadTrigger | 📥 | Generate download |
| FavoriteTrigger | ⭐ | Add to favorites |
| ReadTrigger | 📖 | Open reader |

---

## 11. CONFIGURATION SYSTEM

### Configuration Sources

1. `appsettings.json` - Base configuration
2. `appsettings.Development.json` - Development overrides
3. `appsecrets.json` - Secrets (not in source control)
4. Environment variables

### Configuration Structure

```json
{
  "logging": {
    "logLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  },
  "discord": {
    "prefix": "n!",
    "token": "BOT_TOKEN_HERE",
    "botInvite": "https://discord.com/oauth2/...",
    "status": {
      "updateInterval": 10,
      "games": ["n!help | Version 3.4"]
    },
    "guild": {
      "guildId": 515395714264858653,
      "guildInvite": "https://discord.gg/...",
      "errorChannelId": 529695761449877504
    }
  },
  "http": {
    "userAgent": "nhitomi/3.4"
  },
  "feed": {
    "enabled": false
  }
}
```

### AppSettings Classes

| Class | Properties |
|-------|------------|
| `AppSettings` | Discord, Http, Feed |
| `DiscordSettings` | Prefix, Token, BotInvite, Status, Guild |
| `StatusSettings` | UpdateInterval, Games |
| `GuildSettings` | GuildId, GuildInvite, ErrorChannelId |
| `HttpSettings` | UserAgent, Proxy |
| `FeedSettings` | Enabled, UpdateInterval |

---

## 12. LOCALIZATION & INTERNATIONALIZATION

### Supported Languages

| Code | Language | File |
|------|----------|------|
| `en` | English | `Globalization/Languages/en.json` |
| `id` | Indonesian | `Globalization/Languages/id.json` |
| `ko` | Korean | `Globalization/Languages/ko.json` |

### Implementation

```csharp
// Get localization for current context
var l = context.GetLocalization();

// Use localized string with variables
var message = l["doujin.title", new { name = doujin.PrettyName }];
```

### String Format

Uses SmartFormat.NET for variable interpolation:

```json
{
  "doujin": {
    "title": "{name}",
    "info": "Pages: {pages} | Source: {source}",
    "notFound": "Doujin not found."
  }
}
```

### Language Selection

- Per-guild language preference stored in database
- Fallback to English if translation missing
- Admin command to change server language

---

## 13. EXTERNAL INTEGRATIONS

### Discord API

| Feature | Implementation |
|---------|----------------|
| WebSocket Connection | Discord.Net SocketClient |
| Message Handling | MessageReceived event |
| Reaction Handling | ReactionAdded event |
| Embed Messages | EmbedBuilder |
| Bot Status | SetGameAsync |

### Web Scraping

| Source | Method |
|--------|--------|
| nhentai.net | JSON API |
| hitomi.la | HTML + HtmlAgilityPack |

### HTTP Client

Custom `HttpClientWrapper` with:
- User-Agent configuration
- Proxy support
- Request initialization hooks
- Timeout handling

---

## 14. DATA FLOW ANALYSIS

### Search Flow

```
User: n!search "cute girls"
           │
           ▼
    CommandExecutor
           │
           ▼
   DoujinModule.SearchAsync()
           │
           ▼
   Create DoujinSearchArgs
           │
           ▼
   InteractiveManager.SendAsync()
           │
           ▼
   DoujinListFromQueryMessage
           │
           ▼
   Query Database (LIKE search)
           │
           ▼
   Build Embed with Results
           │
           ▼
   Send to Discord
           │
           ▼
   Register Reaction Triggers
```

### Collection Management Flow

```
User: n!collection favs add nhentai 12345
           │
           ▼
    CommandExecutor
           │
           ▼
   CollectionModule.AddAsync()
           │
           ▼
   Get/Create Collection
           │
           ▼
   Fetch Doujin from Database
           │
           ▼
   Create CollectionRef
           │
           ▼
   Save to Database
           │
           ▼
   Send Confirmation
```

### Feed Channel Flow

```
FeedChannelUpdateService (Background)
           │
           ▼
   Query New Doujins (since LastDoujinId)
           │
           ▼
   For Each FeedChannel:
           │
           ├── Load Tag Whitelist
           │
           ├── Filter by Tags (Any/All)
           │
           ├── Send Matching Doujins
           │
           └── Update LastDoujinId
```

---

## 15. ERROR HANDLING & MONITORING

### Error Reporting

`DiscordErrorReporter` catches unhandled exceptions and:
1. Logs error details
2. Sends to designated error channel
3. Preserves stack traces
4. Includes context information

### Logging

```csharp
"logging": {
  "logLevel": {
    "Default": "Information",
    "Microsoft": "Warning",
    "Discord": "Warning"
  }
}
```

### Monitoring (Debug Command)

The `n!debug` command provides:

| Metric | Description |
|--------|-------------|
| Guild Count | Connected servers |
| Channel Count | Total channels |
| User Count | Total users |
| Handled Messages | Commands processed |
| Received Messages | Total messages |
| Handled Reactions | Reactions processed |
| Received Reactions | Total reactions |
| Interactive Count | Active interactive messages |
| Virtual Memory | Process memory |
| Working Set | Physical memory |
| Managed Memory | .NET heap |
| Framework | Runtime version |

### Counters

Thread-safe `AtomicCounter` class tracks:
- `HandledMessages`
- `ReceivedMessages`
- `HandledReactions`
- `ReceivedReactions`

---

## 16. DEPLOYMENT & CI/CD

### AppVeyor Configuration

```yaml
version: '{build}'
image: Visual Studio 2017

clone_depth: 1

install:
  - git submodule update --init --recursive

build_script:
  - dotnet build
```

### Docker Configuration

```dockerfile
# Build stage
FROM microsoft/dotnet:sdk AS build
WORKDIR /app
COPY . .
RUN dotnet publish -c Release -o out

# Runtime stage
FROM microsoft/dotnet:aspnetcore-runtime
WORKDIR /app
COPY --from=build /app/nhitomi/out .
EXPOSE 80
ENTRYPOINT ["dotnet", "nhitomi.dll"]
```

### Dependency Management

Renovate bot configured for automated updates:

```json
{
  "$schema": "https://docs.renovatebot.com/renovate-schema.json",
  "extends": ["config:base"]
}
```

---

## 17. FILE INVENTORY

### C# Source Files by Category

| Category | Count | Key Files |
|----------|-------|-----------|
| **Core Models** | 9 | Doujin.cs, Tag.cs, Collection.cs, Guild.cs, FeedChannel.cs |
| **Source Clients** | 8 | nhentaiClient.cs, HitomiClient.cs + test cases |
| **Discord Services** | 12 | DiscordService.cs, CommandExecutor.cs, handlers |
| **Command Modules** | 5 | DoujinModule.cs, CollectionModule.cs, etc. |
| **Interactivity** | 15 | InteractiveManager.cs, message types, triggers |
| **Database/EF** | 5 | nhitomiDbContext.cs, migrations |
| **Utilities** | 20+ | Various helper classes |
| **Tests** | 2 | ClientTests.cs, TestUtils.cs |

### Configuration Files

| File | Purpose |
|------|---------|
| nhitomi.sln | Visual Studio solution |
| nhitomi/nhitomi.csproj | Main project |
| nhitomi.Core/nhitomi.Core.csproj | Core library project |
| appveyor.yml | CI/CD configuration |
| Dockerfile | Container configuration |
| renovate.json | Dependency automation |
| appsettings.json | Application configuration |
| appsettings.Development.json | Development overrides |

### Localization Files

| File | Language |
|------|----------|
| en.json | English (default) |
| id.json | Indonesian |
| ko.json | Korean |

---

## 18. DEPENDENCIES

### NuGet Packages (nhitomi)

| Package | Version |
|---------|---------|
| Discord.Net | 2.1.1 |
| Microsoft.AspNetCore | 2.2.0 |
| Microsoft.AspNetCore.Hosting | 2.2.0 |
| Microsoft.EntityFrameworkCore.Design | 2.2.4 |
| Microsoft.Extensions.Configuration.* | 2.2.0 |
| Microsoft.Extensions.Hosting | 2.2.0 |

### NuGet Packages (nhitomi.Core)

| Package | Version |
|---------|---------|
| HtmlAgilityPack | 1.11.7 |
| Microsoft.EntityFrameworkCore | 2.2.4 |
| Microsoft.EntityFrameworkCore.Sqlite | 2.2.4 |
| Newtonsoft.Json | 12.0.2 |
| Pomelo.EntityFrameworkCore.MySql | 2.2.0 |
| SmartFormat.NET | 2.4.2 |
| System.Interactive.Async | 3.2.0 |

### NuGet Packages (Tests)

| Package | Version |
|---------|---------|
| Microsoft.NET.Test.Sdk | 16.0.1 |
| xunit | 2.4.1 |
| xunit.runner.visualstudio | 2.4.1 |

---

## 19. SECURITY CONSIDERATIONS

### Identified Concerns

| Area | Concern | Recommendation |
|------|---------|----------------|
| **Token Storage** | Discord token in config file | Use environment variables or secret manager |
| **SQL Injection** | EF Core parameterizes queries | Current implementation is safe |
| **Input Validation** | Command inputs need validation | Add input sanitization |
| **Rate Limiting** | No explicit rate limiting | Implement rate limiting for commands |
| **Permissions** | Guild admin checks needed | Verify permission checks on admin commands |
| **Data Exposure** | User collections are personal | Ensure collection privacy |

### Current Security Measures

- EF Core prevents SQL injection
- Discord.Net handles token security
- Environment-specific configuration
- Secrets file excluded from source control

---

## 20. RECOMMENDATIONS

### Code Quality

1. **Update .NET Version** - .NET Core 2.2 is end-of-life; upgrade to .NET 6+ or .NET 8
2. **Add Unit Tests** - Current test coverage is minimal
3. **Implement Logging** - Add structured logging with Serilog or similar
4. **Documentation** - Add XML documentation comments

### Architecture

1. **Implement CQRS** - Separate read/write operations for better scaling
2. **Add Caching** - Cache frequently accessed data (doujins, tags)
3. **Message Queue** - Use queue for feed distribution
4. **Health Checks** - Add health check endpoints

### Security

1. **Rate Limiting** - Implement per-user rate limits
2. **Audit Logging** - Log admin actions
3. **Permission System** - Formalize permission checks

### Operations

1. **Container Orchestration** - Consider Kubernetes deployment
2. **Monitoring** - Add Application Insights or Prometheus
3. **Backup Strategy** - Implement automated database backups
4. **Scaling** - Design for horizontal scaling

### Feature Additions

1. **More Sources** - Add additional doujin sources
2. **Improved Search** - Implement Elasticsearch for better search
3. **User Profiles** - Add user statistics and preferences
4. **Web Interface** - Consider adding a web dashboard

---

## APPENDIX A: GLOSSARY

| Term | Definition |
|------|------------|
| Doujin/Doujinshi | Self-published Japanese works, often manga |
| nhentai | Popular doujinshi hosting website |
| Hitomi | Alternative doujinshi hosting website |
| EF Core | Entity Framework Core - .NET ORM |
| Discord.Net | C# library for Discord API |
| Feed Channel | Auto-posting channel for new content |
| Interactive Message | Discord message with reaction controls |

---

## APPENDIX B: COMMAND QUICK REFERENCE

```
BROWSING
n!get <source> <id>     - View doujin details
n!from <source>         - Browse source
n!search <query>        - Search doujins
n!read <source> <id>    - Open reader
n!download <source> <id> - Get download link

COLLECTIONS
n!collections           - List your collections
n!collection <name>     - View collection
n!collection <name> add <source> <id>    - Add to collection
n!collection <name> remove <source> <id> - Remove from collection
n!collection <name> sort <field>         - Sort collection
n!collection <name> delete               - Delete collection

SETTINGS
n!language <code>       - Set language (en/id/ko)
n!quality               - Toggle quality filter

HELP
n!help                  - Show help
n!debug                 - Show debug stats
```

---

## APPENDIX C: ENVIRONMENT VARIABLES

| Variable | Description |
|----------|-------------|
| `ASPNETCORE_ENVIRONMENT` | Environment name (Development/Production) |
| `Discord__Token` | Discord bot token |
| `ConnectionStrings__Default` | Database connection string |

---

*Report generated by Claude Opus 4.5*
*Analysis completed: 2026-02-03*
