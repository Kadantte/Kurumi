using System.Collections.Concurrent;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using nhitomi.Discord;
using nhitomi.Interactivity.Triggers;

namespace nhitomi.Interactivity;

public class InteractiveManager : IMessageHandler, IReactionHandler
{
    private readonly IServiceProvider _services;

    public InteractiveManager(IServiceProvider services)
    {
        _services = services;
    }

    public readonly ConcurrentDictionary<ulong, IInteractiveMessage> InteractiveMessages = new();

    public async Task SendInteractiveAsync(
        IEmbedMessage message,
        IDiscordContext context,
        CancellationToken cancellationToken = default,
        bool forceStateful = true)
    {
        // Create dependency scope to initialize the interactive within
        using (var scope = _services.CreateScope())
        {
            var services = new ServiceDictionary(scope.ServiceProvider)
            {
                { typeof(IDiscordContext), context }
            };

            // Initialize interactive
            if (!await message.UpdateViewAsync(services, cancellationToken))
                return;
        }

        if (message.Message == null)
            return;

        var id = message.Message.Id;

        if (message is IInteractiveMessage interactiveMessage)
            if (forceStateful || interactiveMessage.Triggers.Values.Any(t => !t.CanRunStateless))
                InteractiveMessages[id] = interactiveMessage;

        // Forget interactives in an hour
        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(TimeSpan.FromHours(1), cancellationToken);
            }
            catch (TaskCanceledException) { }
            finally
            {
                InteractiveMessages.TryRemove(id, out _);
            }
        }, cancellationToken);
    }

    private readonly ConcurrentQueue<(IUserMessage message, IEmote[] emotes)> _reactionQueue = new();

    public void EnqueueReactions(IUserMessage message, IEnumerable<IEmote> emotes)
    {
        _reactionQueue.Enqueue((message, emotes.ToArray()));

        _ = Task.Run(async () =>
        {
            while (_reactionQueue.TryDequeue(out var x))
            {
                try
                {
                    await x.message.AddReactionsAsync(x.emotes);
                }
                catch
                {
                    // Message may have been deleted or we don't have the perms
                }
            }
        });
    }

    private static readonly Dictionary<IEmote, Func<IReactionTrigger>> StatelessTriggers =
        typeof(Startup)
            .Assembly
            .GetTypes()
            .Where(t => t.IsClass &&
                        !t.IsAbstract &&
                        typeof(IReactionTrigger).IsAssignableFrom(t) &&
                        t.GetConstructors().Any(c => c.GetParameters().Length == 0))
            .Select(t => (Func<IReactionTrigger>)(() => (Activator.CreateInstance(t) as IReactionTrigger)!))
            .Where(x => x().CanRunStateless)
            .ToDictionary(x => x().Emote, x => x);

    Task IMessageHandler.InitializeAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    Task IReactionHandler.InitializeAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task<bool> TryHandleAsync(IMessageContext context, CancellationToken cancellationToken = default)
    {
        switch (context.Event)
        {
            case MessageEvent.Delete when context.Message != null &&
                                          InteractiveMessages.TryRemove(context.Message.Id, out var interactive):
                interactive.Dispose();
                return Task.FromResult(true);

            default:
                return Task.FromResult(false);
        }
    }

    public async Task<bool> TryHandleAsync(IReactionContext context, CancellationToken cancellationToken = default)
    {
        var message = context.Message;
        var reaction = context.Reaction;

        if (message == null)
            return false;

        IReactionTrigger? trigger;

        // Get interactive object for the message
        if (InteractiveMessages.TryGetValue(message.Id, out var interactive))
        {
            // Get trigger for this reaction
            if (!interactive.Triggers.TryGetValue(reaction.Emote, out trigger))
                return false;
        }
        else
        {
            // No interactive; try triggering in stateless mode
            if (!StatelessTriggers.TryGetValue(reaction.Emote, out var factory))
                return false;

            // Message must be authored by us
            if (!message.Reactions.TryGetValue(reaction.Emote, out var metadata) || !metadata.IsMe)
                return false;

            trigger = factory();
        }

        // Dependency scope
        using (var scope = _services.CreateScope())
        {
            var services = new ServiceDictionary(scope.ServiceProvider)
            {
                { typeof(IDiscordContext), context },
                { typeof(IReactionContext), context }
            };

            return await trigger.RunAsync(services, interactive, cancellationToken);
        }
    }
}
