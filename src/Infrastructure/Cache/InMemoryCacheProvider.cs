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

        public async Task<T?> GetAsync<T>(string key)
        {
            return await Task.FromResult(_cache.Get<T>(key));
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expirationTime = null)
        {
            var options = new MemoryCacheEntryOptions();
            if (expirationTime.HasValue)
            {
                options.SetSlidingExpiration(expirationTime.Value);
            }

            _cache.Set(key, value, options);
            await Task.CompletedTask;
        }

        public async Task RemoveAsync(string key)
        {
            _cache.Remove(key);
            await Task.CompletedTask;
        }
    }
}