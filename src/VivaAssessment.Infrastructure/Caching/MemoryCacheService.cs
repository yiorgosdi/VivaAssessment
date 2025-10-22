using Microsoft.Extensions.Caching.Memory;
using VivaAssessment.Application.Abstractions;

namespace VivaAssessment.Infrastructure.Caching;

public sealed class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    public MemoryCacheService(IMemoryCache cache) => _cache = cache;

    public T? Get<T>(string key) => _cache.TryGetValue(key, out T? value) ? value : default;

    public void Set<T>(string key, T value, TimeSpan ttl)
    {
        var opts = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl
        };
        _cache.Set(key, value, opts);
    }

    public void Remove(string key) => _cache.Remove(key);
}
