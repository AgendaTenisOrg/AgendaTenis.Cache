using Microsoft.Extensions.Caching.Distributed;

namespace AgendaTenis.Cache.Core;

public static class RedisExtensoes
{
    public static async Task SetRecordAsync<T>(
       this IDistributedCache cache,
       string recordId,
       T data,
       TimeSpan? absoluteExpireTime = null,
       TimeSpan? unusedExpireTime = null)
    {
        var options = new DistributedCacheEntryOptions();
        options.AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromSeconds(60);
        options.SlidingExpiration = unusedExpireTime;

        var jsonData = System.Text.Json.JsonSerializer.Serialize(data);
        await cache.SetStringAsync(recordId, jsonData, options);
    }

    public static async Task<T> GetRecordAsync<T>(this IDistributedCache cache, string recordId)
    {
        var jsonData = await cache.GetStringAsync(recordId);

        if (jsonData is null)
            return default;

        return System.Text.Json.JsonSerializer.Deserialize<T>(jsonData);
    }
}
