# nhitomi Configuration Guide

This document describes all configuration options for the nhitomi Discord bot.

## Configuration Sources

Configuration is loaded in the following order (later sources override earlier ones):

1. `appsettings.json` - Base configuration
2. `appsettings.{Environment}.json` - Environment-specific overrides
3. Environment variables - Runtime configuration

## Environment Variables

All configuration keys can be set via environment variables using the `__` (double underscore) separator for nested keys.

### Required Variables

| Variable | Description | Example |
|----------|-------------|---------|
| `Discord__Token` | Discord bot token from Developer Portal | `MTIz...` |
| `ConnectionStrings__nhitomi` | MySQL connection string (production) | `Server=localhost;Database=nhitomi;User=root;Password=pass;` |

### Optional Discord Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `Discord__BotInvite` | OAuth2 invite URL for the bot | - |
| `Discord__TestGuildId` | Guild ID for testing slash commands | - (global registration) |
| `Discord__Guild__GuildId` | Home guild ID | - |
| `Discord__Guild__GuildInvite` | Invite link to home guild | - |
| `Discord__Guild__ErrorChannelId` | Channel for error reports | - |
| `Discord__Status__UpdateInterval` | Status rotation interval (seconds) | `60` |
| `Discord__Status__Games__0` | First status game text | - |
| `Discord__Status__Games__1` | Second status game text | - |

### HTTP Client Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `Http__EnableProxy` | Enable HTTP proxy | `false` |
| `Http__TimeoutSeconds` | HTTP request timeout | `30` |
| `Http__RetryCount` | Number of retry attempts | `3` |
| `Http__RetryDelayMilliseconds` | Base delay between retries | `500` |

### Feed Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `Feed__Enabled` | Enable feed channel functionality | `false` |

### Logging Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `Logging__LogLevel__Default` | Default log level | `Information` |
| `Logging__LogLevel__Microsoft` | Microsoft framework log level | `Warning` |
| `Logging__LogLevel__Discord` | Discord.Net log level | `Information` |

## Example Configuration Files

### appsettings.json (Development)

```json
{
  "Discord": {
    "Token": null,
    "TestGuildId": 123456789012345678,
    "Status": {
      "UpdateInterval": 60,
      "Games": ["with slash commands", "v3.4 Heresta"]
    },
    "Guild": {
      "GuildId": 0,
      "GuildInvite": null,
      "ErrorChannelId": 0
    }
  },
  "ConnectionStrings": {
    "nhitomi": null
  },
  "Http": {
    "EnableProxy": false,
    "TimeoutSeconds": 30,
    "RetryCount": 3,
    "RetryDelayMilliseconds": 500
  },
  "Feed": {
    "Enabled": false
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Warning"
    }
  }
}
```

### appsettings.Production.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  },
  "Feed": {
    "Enabled": true
  }
}
```

## Docker Configuration

When running in Docker, use environment variables:

```bash
docker run -d \
  -e Discord__Token=your_token_here \
  -e ConnectionStrings__nhitomi="Server=db;Database=nhitomi;User=app;Password=secret;" \
  -e Discord__Guild__GuildId=123456789012345678 \
  -e Feed__Enabled=true \
  -p 8080:8080 \
  nhitomi
```

## Docker Compose Example

```yaml
version: '3.8'
services:
  nhitomi:
    build: .
    environment:
      - Discord__Token=${DISCORD_TOKEN}
      - ConnectionStrings__nhitomi=Server=db;Database=nhitomi;User=app;Password=${DB_PASSWORD};
      - Discord__Guild__GuildId=${GUILD_ID}
      - Feed__Enabled=true
    ports:
      - "8080:8080"
    depends_on:
      - db
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/health"]
      interval: 30s
      timeout: 10s
      retries: 3

  db:
    image: mysql:8.0
    environment:
      - MYSQL_DATABASE=nhitomi
      - MYSQL_USER=app
      - MYSQL_PASSWORD=${DB_PASSWORD}
      - MYSQL_ROOT_PASSWORD=${DB_ROOT_PASSWORD}
    volumes:
      - mysql_data:/var/lib/mysql

volumes:
  mysql_data:
```

## Startup Validation

In production environments (`DOTNET_ENVIRONMENT` != `Development`), the following settings are validated at startup:

- `Discord:Token` - Must be provided
- `Http:TimeoutSeconds` - Must be positive
- `Http:RetryCount` - Must be non-negative
- `Http:RetryDelayMilliseconds` - Must be non-negative

Missing or invalid configuration will cause the application to fail fast with a clear error message.

## Health Check Endpoints

The bot exposes health check endpoints on port 8080:

| Endpoint | Description |
|----------|-------------|
| `/health` | Basic health status (Discord connection) |
| `/ready` | Readiness check (connected and guilds loaded) |
| `/metrics` | Runtime metrics (memory, GC, Discord stats) |

## Secrets Management

**Important**: Never commit secrets to version control.

Recommended approaches for secrets:
1. Use environment variables (Docker/Kubernetes)
2. Use a secrets manager (Azure Key Vault, AWS Secrets Manager)
3. Use `.env` files locally (ensure `.env` is in `.gitignore`)

The Discord token and database password should always be provided via environment variables or a secrets manager, not in configuration files.
