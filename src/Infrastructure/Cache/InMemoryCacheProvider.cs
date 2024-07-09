using Application.Abstraction.Cache;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Cache
{
    public class InMemoryCacheProvider : ICacheProvider
    {
        private readonly IMemoryCache _cache;

        public InMemoryCacheProvider(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> acquire, TimeSpan expiration)
        {
            if (!_cache.TryGetValue(key, out T value))
            {
                value = await acquire();
                _cache.Set(key, value, new MemoryCacheEntryOptions().SetSlidingExpiration(expiration));
            }
            return value;
        }

        public void Remove(string key)
        {

        }
    }
}