using Discord;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace nhitomi.Discord;

public class StatusUpdateService : BackgroundService
{
    private readonly AppSettings _settings;
    private readonly DiscordService _discord;

    public StatusUpdateService(
        IOptions<AppSettings> options,
        DiscordService discord)
    {
        _settings = options.Value;
        _discord = discord;
    }

    private readonly Random _rand = new();
    private string? _current;

    private void CycleGame()
    {
        var games = _settings.Discord.Status.Games;
        if (games.Length == 0)
            return;

        var index = _current == null ? -1 : Array.IndexOf(games, _current);
        int next;

        // Keep choosing if we chose the same one
        do
        {
            next = _rand.Next(games.Length);
        }
        while (next == index && games.Length > 1);

        // Updated to use slash commands hint instead of prefix
        _current = $"{games[next]} [/help]";
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _discord.WaitForReadyAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            CycleGame();

            if (_current != null)
            {
                // Send update
                await _discord.Client.SetActivityAsync(new Game(_current, ActivityType.Playing));
            }

            // Sleep
            await Task.Delay(TimeSpan.FromMinutes(_settings.Discord.Status.UpdateInterval), stoppingToken);
        }
    }
}
