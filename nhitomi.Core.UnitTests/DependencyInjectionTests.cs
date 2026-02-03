using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using nhitomi;
using nhitomi.Discord;
using nhitomi.Interactivity;
using NUnit.Framework;

namespace nhitomi.Core.UnitTests;

[TestFixture]
public class DependencyInjectionTests
{
    private IHost? _host;

    [SetUp]
    public void SetUp()
    {
        var builder = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Discord:Token"] = "test-token",
                    ["Discord:TestGuildId"] = "123456789"
                });
            })
            .ConfigureServices((context, services) =>
            {
                Startup.ConfigureServices(services, context.Configuration);
            });

        _host = builder.Build();
    }

    [TearDown]
    public void TearDown()
    {
        _host?.Dispose();
    }

    [Test]
    public void CanResolveDiscordSocketClient()
    {
        var client = _host!.Services.GetService<DiscordSocketClient>();
        Assert.That(client, Is.Not.Null);
    }

    [Test]
    public void CanResolveInteractionService()
    {
        var service = _host!.Services.GetService<InteractionService>();
        Assert.That(service, Is.Not.Null);
    }

    [Test]
    public void CanResolveDiscordService()
    {
        var service = _host!.Services.GetService<DiscordService>();
        Assert.That(service, Is.Not.Null);
    }

    [Test]
    public void CanResolveInteractiveManager()
    {
        var manager = _host!.Services.GetService<InteractiveManager>();
        Assert.That(manager, Is.Not.Null);
    }

    [Test]
    public void CanResolveGuildSettingsCache()
    {
        var cache = _host!.Services.GetService<GuildSettingsCache>();
        Assert.That(cache, Is.Not.Null);
    }

    [Test]
    public void CanResolveGalleryUrlDetector()
    {
        var detector = _host!.Services.GetService<GalleryUrlDetector>();
        Assert.That(detector, Is.Not.Null);
    }

    [Test]
    public void CanResolveDiscordErrorReporter()
    {
        var reporter = _host!.Services.GetService<DiscordErrorReporter>();
        Assert.That(reporter, Is.Not.Null);
    }

    [Test]
    public void AllHostedServicesAreRegistered()
    {
        var hostedServices = _host!.Services.GetServices<IHostedService>().ToList();

        Assert.That(hostedServices, Is.Not.Empty);

        var serviceTypes = hostedServices.Select(s => s.GetType().Name).ToList();

        Assert.That(serviceTypes, Does.Contain(nameof(MessageHandlerService)));
        Assert.That(serviceTypes, Does.Contain(nameof(ReactionHandlerService)));
        Assert.That(serviceTypes, Does.Contain(nameof(StatusUpdateService)));
        Assert.That(serviceTypes, Does.Contain(nameof(LogHandlerService)));
        Assert.That(serviceTypes, Does.Contain(nameof(GuildSettingsSyncService)));
        Assert.That(serviceTypes, Does.Contain(nameof(FeedChannelUpdateService)));
        Assert.That(serviceTypes, Does.Contain(nameof(GuildWelcomeMessageService)));
    }

    [Test]
    public void HostedServicesAreResolvableAsSingletons()
    {
        var messageHandler1 = _host!.Services.GetService<MessageHandlerService>();
        var messageHandler2 = _host!.Services.GetService<MessageHandlerService>();

        Assert.That(messageHandler1, Is.Not.Null);
        Assert.That(messageHandler2, Is.Not.Null);
        Assert.That(messageHandler1, Is.SameAs(messageHandler2), "MessageHandlerService should be a singleton");

        var reactionHandler1 = _host!.Services.GetService<ReactionHandlerService>();
        var reactionHandler2 = _host!.Services.GetService<ReactionHandlerService>();

        Assert.That(reactionHandler1, Is.Not.Null);
        Assert.That(reactionHandler2, Is.Not.Null);
        Assert.That(reactionHandler1, Is.SameAs(reactionHandler2), "ReactionHandlerService should be a singleton");
    }

    [Test]
    public void DatabaseIsResolvable()
    {
        using var scope = _host!.Services.CreateScope();
        var database = scope.ServiceProvider.GetService<IDatabase>();
        Assert.That(database, Is.Not.Null);
    }
}
