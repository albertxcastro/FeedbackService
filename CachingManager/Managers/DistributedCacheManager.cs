using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CachingManager.Managers
{
    public class DistributedCacheManager : IDistributedCacheManager
    {
        private readonly IDistributedCache _distributedCache;

        public DistributedCacheManager(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public T GetFromCache<T>(string key)
        {
            var cachedMessage = _distributedCache.GetString(key);
            if(cachedMessage == null)
            {
                return default;
            }

            return (T)JsonSerializer.Deserialize(cachedMessage, typeof(T));
        }

        public byte[] GetFromCache(string key)
        {
            return _distributedCache.Get(key);
        }

        public async Task<T> GetFromCacheAsync<T>(string key, CancellationToken cancellationToken)
        {
            var cachedMessage = await _distributedCache.GetStringAsync(key, cancellationToken);
            if (cachedMessage == null)
            {
                return default;
            }
            return (T)JsonSerializer.Deserialize(cachedMessage, typeof(T));
        }

        public async Task<byte[]> GetFromCacheAsync(string key, CancellationToken cancellationToken)
        {
            return await _distributedCache.GetAsync(key, cancellationToken);
        }

        public void Remove(string key)
        {
            _distributedCache.Remove(key);
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken)
        {
            await _distributedCache.RemoveAsync(key, cancellationToken);
        }

        public void SetCache<T>(string key, T message, DistributedCacheEntryOptions options)
        {
            _distributedCache.SetString(key, JsonSerializer.Serialize(message, typeof(T)), options);
        }

        public void SetCache(string key, byte[] message, DistributedCacheEntryOptions options)
        {
            _distributedCache.Set(key, message, options);
        }

        public async Task SetCacheAsync<T>(string key, T message, DistributedCacheEntryOptions options, CancellationToken cancellationToken)
        {
            await _distributedCache.SetStringAsync(key, JsonSerializer.Serialize(message), options, cancellationToken);
        }

        public async Task SetCacheAsync(string key, byte[] message, DistributedCacheEntryOptions options, CancellationToken cancellationToken)
        {
            await _distributedCache.SetAsync(key, message, options, cancellationToken);
        }
    }
}
