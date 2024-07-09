using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace AgendaTenis.Cache.Core;

public static class RedisExtensoes
{
    public static async Task SetRecordAsync<T>(
       this IDistributedCache cache,
       string recordId,
       T data,
       TimeSpan? absoluteExpireTime = null,
       TimeSpan? unusedExpireTime = null,
       bool lancarException = true)
    {
        var options = new DistributedCacheEntryOptions();
        options.AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromSeconds(60);
        options.SlidingExpiration = unusedExpireTime;

        var jsonData = System.Text.Json.JsonSerializer.Serialize(data);

        try
        {
            await cache.SetStringAsync(recordId, jsonData, options);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Não foi possível adicionar registro ao cache. {ex}");

            if (lancarException)
                throw;
        }
    }

    public static async Task<T> GetRecordAsync<T>(this IDistributedCache cache, string recordId, bool lancarException = true)
    {
        string? jsonData = null;

        try
        {
            jsonData = await cache.GetStringAsync(recordId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Não foi possível obter registro do cache. {ex}");

            if (lancarException)
                throw;
        }

        if (jsonData is null)
            return default;

        return System.Text.Json.JsonSerializer.Deserialize<T>(jsonData);
    }
}
