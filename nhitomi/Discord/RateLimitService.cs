using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace nhitomi.Discord;

/// <summary>
/// Provides per-user and per-guild rate limiting for command execution.
/// </summary>
public class RateLimitService
{
    private readonly ILogger<RateLimitService> _logger;

    // Key: "{type}:{id}", Value: (count, windowStart)
    private readonly ConcurrentDictionary<string, RateLimitBucket> _buckets = new();

    // Default limits
    private const int DefaultUserLimit = 10;       // commands per window
    private const int DefaultGuildLimit = 50;      // commands per window
    private const int WindowSeconds = 60;          // sliding window

    public RateLimitService(ILogger<RateLimitService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Checks if a user has exceeded their rate limit.
    /// </summary>
    public bool IsUserRateLimited(ulong userId, out TimeSpan retryAfter)
    {
        return IsRateLimited($"user:{userId}", DefaultUserLimit, out retryAfter);
    }

    /// <summary>
    /// Checks if a guild has exceeded its rate limit.
    /// </summary>
    public bool IsGuildRateLimited(ulong guildId, out TimeSpan retryAfter)
    {
        return IsRateLimited($"guild:{guildId}", DefaultGuildLimit, out retryAfter);
    }

    /// <summary>
    /// Records a command execution for rate limiting.
    /// </summary>
    public void RecordUserCommand(ulong userId)
    {
        RecordCommand($"user:{userId}", DefaultUserLimit);
    }

    /// <summary>
    /// Records a command execution for guild rate limiting.
    /// </summary>
    public void RecordGuildCommand(ulong guildId)
    {
        RecordCommand($"guild:{guildId}", DefaultGuildLimit);
    }

    private bool IsRateLimited(string key, int limit, out TimeSpan retryAfter)
    {
        retryAfter = TimeSpan.Zero;

        var bucket = _buckets.GetOrAdd(key, _ => new RateLimitBucket());
        var now = DateTimeOffset.UtcNow;

        lock (bucket)
        {
            // Reset bucket if window has passed
            if (now - bucket.WindowStart > TimeSpan.FromSeconds(WindowSeconds))
            {
                bucket.Count = 0;
                bucket.WindowStart = now;
            }

            if (bucket.Count >= limit)
            {
                retryAfter = bucket.WindowStart.AddSeconds(WindowSeconds) - now;
                _logger.LogDebug("Rate limited {Key}: {Count}/{Limit}, retry after {RetryAfter}",
                    key, bucket.Count, limit, retryAfter);
                return true;
            }

            return false;
        }
    }

    private void RecordCommand(string key, int limit)
    {
        var bucket = _buckets.GetOrAdd(key, _ => new RateLimitBucket());
        var now = DateTimeOffset.UtcNow;

        lock (bucket)
        {
            // Reset bucket if window has passed
            if (now - bucket.WindowStart > TimeSpan.FromSeconds(WindowSeconds))
            {
                bucket.Count = 0;
                bucket.WindowStart = now;
            }

            bucket.Count++;

            _logger.LogDebug("Recorded command for {Key}: {Count}/{Limit}",
                key, bucket.Count, limit);
        }
    }

    /// <summary>
    /// Cleans up expired buckets to prevent memory leaks.
    /// Should be called periodically.
    /// </summary>
    public void CleanupExpiredBuckets()
    {
        var now = DateTimeOffset.UtcNow;
        var expiredKeys = _buckets
            .Where(kvp =>
            {
                lock (kvp.Value)
                {
                    return now - kvp.Value.WindowStart > TimeSpan.FromSeconds(WindowSeconds * 2);
                }
            })
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in expiredKeys)
        {
            _buckets.TryRemove(key, out _);
        }

        if (expiredKeys.Count > 0)
        {
            _logger.LogDebug("Cleaned up {Count} expired rate limit buckets", expiredKeys.Count);
        }
    }

    private class RateLimitBucket
    {
        public int Count { get; set; }
        public DateTimeOffset WindowStart { get; set; } = DateTimeOffset.UtcNow;
    }
}
