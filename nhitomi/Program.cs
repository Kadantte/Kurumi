using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using nhitomi.Core;
using nhitomi.Discord;

namespace nhitomi;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        Startup.Configure(builder.Configuration, builder.Environment);

        // Validate configuration before proceeding
        var settings = builder.Configuration.Get<AppSettings>() ?? new AppSettings();
        if (!builder.Environment.IsDevelopment())
        {
            settings.ValidateAndThrow();
        }

        Startup.ConfigureServices(builder.Services, builder.Configuration);

        using var host = builder.Build();

        // Initialization
        using (var scope = host.Services.CreateScope())
        {
            var initialization = new Initialization(
                scope.ServiceProvider.GetRequiredService<IHostEnvironment>(),
                scope.ServiceProvider.GetRequiredService<nhitomiDbContext>(),
                scope.ServiceProvider.GetRequiredService<DiscordService>()
            );
            await initialization.RunAsync();
        }

        await host.RunAsync();
    }

    private sealed class Initialization
    {
        private readonly IHostEnvironment _environment;
        private readonly nhitomiDbContext _db;
        private readonly DiscordService _discord;

        public Initialization(
            IHostEnvironment environment,
            nhitomiDbContext db,
            DiscordService discord)
        {
            _environment = environment;
            _db = db;
            _discord = discord;
        }

        public async Task RunAsync(CancellationToken cancellationToken = default)
        {
            // Migrate database for development
            if (_environment.IsDevelopment())
                await _db.Database.MigrateAsync(cancellationToken);

            // Start discord and register slash commands
            await _discord.ConnectAsync();
            await _discord.RegisterCommandsAsync();
        }
    }
}
