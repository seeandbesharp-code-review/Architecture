using ApiProject.Services.Interface;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace ApiProject.Services.Implement
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<CacheService> _logger;

        public CacheService(IDistributedCache cache, ILogger<CacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                var json = await _cache.GetStringAsync(key);

                // אם לא נמצא ב-Cache (Cache Miss) → מחזיר null
                if (json == null)
                {
                    _logger.LogDebug("Cache MISS for key: {Key}", key);
                    return default;
                }

                _logger.LogDebug("Cache HIT for key: {Key}", key);
                return JsonSerializer.Deserialize<T>(json);
            }
            catch (Exception ex)
            {
                // אם Redis לא זמין — ממשיכים בלי Cache (לא קורסים)
                _logger.LogWarning(ex, "Redis unavailable for GET key: {Key}. Falling back to DB.", key);
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan ttl)
        {
            try
            {
                var json = JsonSerializer.Serialize(value);

                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = ttl  // TTL — אחרי הזמן הזה Redis מוחק אוטומטית
                };

                await _cache.SetStringAsync(key, json, options);
                _logger.LogDebug("Cached key: {Key} with TTL: {Ttl}s", key, ttl.TotalSeconds);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis unavailable for SET key: {Key}. Data not cached.", key);
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                await _cache.RemoveAsync(key);
                _logger.LogDebug("Cache invalidated for key: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis unavailable for REMOVE key: {Key}.", key);
            }
        }
    }
}
