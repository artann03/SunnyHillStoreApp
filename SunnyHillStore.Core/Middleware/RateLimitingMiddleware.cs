using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

public class RateLimitingMiddleware
{
    private static readonly Dictionary<string, TokenBucket> _buckets = new();
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private const int MaxRequests = 100;
    private const int RefillRate = 10;
    private const int RefillTimeInSeconds = 1;

    public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        
        if (!_buckets.TryGetValue(ipAddress, out var bucket))
        {
            bucket = new TokenBucket(MaxRequests, RefillRate, RefillTimeInSeconds);
            _buckets[ipAddress] = bucket;
        }

        if (!bucket.TryTake())
        {
            _logger.LogWarning("Rate limit exceeded for IP: {IpAddress}", ipAddress);
            context.Response.StatusCode = 429;
            await context.Response.WriteAsJsonAsync(new { error = "Too many requests" });
            return;
        }

        await _next(context);
    }

    private class TokenBucket
    {
        private readonly int _capacity;
        private readonly int _refillRate;
        private readonly int _refillTimeInSeconds;
        private int _tokens;
        private DateTime _lastRefill;

        public TokenBucket(int capacity, int refillRate, int refillTimeInSeconds)
        {
            _capacity = capacity;
            _refillRate = refillRate;
            _refillTimeInSeconds = refillTimeInSeconds;
            _tokens = capacity;
            _lastRefill = DateTime.UtcNow;
        }

        public bool TryTake()
        {
            RefillTokens();
            if (_tokens <= 0) return false;
            _tokens--;
            return true;
        }

        private void RefillTokens()
        {
            var now = DateTime.UtcNow;
            var timePassed = (now - _lastRefill).TotalSeconds;
            var tokensToAdd = (int)(timePassed / _refillTimeInSeconds * _refillRate);
            if (tokensToAdd > 0)
            {
                _tokens = Math.Min(_capacity, _tokens + tokensToAdd);
                _lastRefill = now;
            }
        }
    }
} 