using System.Net;
using System.Text;
using System.Text.Json;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace nhitomi.Discord;

/// <summary>
/// Provides a simple HTTP health check endpoint for container orchestration.
/// </summary>
public class HealthCheckService : BackgroundService
{
    private readonly DiscordService _discord;
    private readonly IServiceProvider _services;
    private readonly ILogger<HealthCheckService> _logger;
    private readonly AppSettings _settings;
    private HttpListener? _listener;

    public HealthCheckService(
        DiscordService discord,
        IServiceProvider services,
        IOptions<AppSettings> options,
        ILogger<HealthCheckService> logger)
    {
        _discord = discord;
        _services = services;
        _settings = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _listener = new HttpListener();
        _listener.Prefixes.Add("http://+:8080/");

        try
        {
            _listener.Start();
            _logger.LogInformation("Health check endpoint started on port 8080");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var context = await _listener.GetContextAsync().WaitAsync(stoppingToken);
                    _ = HandleRequestAsync(context, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error handling health check request");
                }
            }
        }
        catch (HttpListenerException ex) when (ex.ErrorCode == 5)
        {
            _logger.LogWarning("Health check endpoint requires admin privileges or URL reservation. Health checks disabled.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start health check endpoint");
        }
    }

    private async Task HandleRequestAsync(HttpListenerContext context, CancellationToken cancellationToken)
    {
        var request = context.Request;
        var response = context.Response;

        try
        {
            var path = request.Url?.AbsolutePath ?? "/";

            var (statusCode, body) = path switch
            {
                "/health" => GetHealthStatus(),
                "/ready" => GetReadyStatus(),
                "/metrics" => GetMetrics(),
                _ => (404, """{"error": "Not found"}""")
            };

            response.StatusCode = statusCode;
            response.ContentType = "application/json";

            var buffer = Encoding.UTF8.GetBytes(body);
            response.ContentLength64 = buffer.Length;

            await response.OutputStream.WriteAsync(buffer, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error processing health check request");
            response.StatusCode = 500;
        }
        finally
        {
            response.Close();
        }
    }

    private (int statusCode, string body) GetHealthStatus()
    {
        var client = _discord.Client;
        var isHealthy = client.ConnectionState == ConnectionState.Connected;

        var status = new
        {
            status = isHealthy ? "healthy" : "unhealthy",
            timestamp = DateTimeOffset.UtcNow,
            discord = new
            {
                connected = client.ConnectionState == ConnectionState.Connected,
                latency = client.Latency,
                state = client.ConnectionState.ToString()
            }
        };

        return (isHealthy ? 200 : 503, JsonSerializer.Serialize(status));
    }

    private (int statusCode, string body) GetReadyStatus()
    {
        var client = _discord.Client;
        var isReady = client.ConnectionState == ConnectionState.Connected &&
                      client.Guilds.Count > 0;

        var status = new
        {
            ready = isReady,
            timestamp = DateTimeOffset.UtcNow,
            guilds = client.Guilds.Count
        };

        return (isReady ? 200 : 503, JsonSerializer.Serialize(status));
    }

    private (int statusCode, string body) GetMetrics()
    {
        var client = _discord.Client;

        var metrics = new
        {
            timestamp = DateTimeOffset.UtcNow,
            discord = new
            {
                guilds = client.Guilds.Count,
                channels = client.Guilds.Sum(g => g.TextChannels.Count),
                users = client.Guilds.Sum(g => g.MemberCount),
                latency = client.Latency
            },
            process = new
            {
                memoryMB = GC.GetTotalMemory(false) / (1024 * 1024),
                gcGen0 = GC.CollectionCount(0),
                gcGen1 = GC.CollectionCount(1),
                gcGen2 = GC.CollectionCount(2),
                threadCount = Environment.ProcessorCount
            }
        };

        return (200, JsonSerializer.Serialize(metrics));
    }

    public override void Dispose()
    {
        _listener?.Close();
        base.Dispose();
    }
}
