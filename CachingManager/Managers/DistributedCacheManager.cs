using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CachingManager.Managers
{
    public class DistributedCacheManager : IDistributedCacheManager
    {
        private readonly IDistributedCache _distributedCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="DistributedCacheManager"/> class.
        /// </summary>
        /// <param name="distributedCache">An instance of an object that implements <see cref="IDistributedCache"/> interface.</param>
        public DistributedCacheManager(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        /// <summary>
        /// Implementation of <see cref="IDistributedCacheManager.GetFromCache{T}(string)"/>.
        /// </summary>
        public T GetFromCache<T>(string key)
        {
            var cachedMessage = _distributedCache.GetString(key);
            if(cachedMessage == null)
            {
                return default;
            }

            return (T)JsonSerializer.Deserialize(cachedMessage, typeof(T));
        }

        /// <summary>
        /// Implementation of <see cref="IDistributedCacheManager.GetFromCache(string)"/>.
        /// </summary>
        public byte[] GetFromCache(string key)
        {
            return _distributedCache.Get(key);
        }

        /// <summary>
        /// Implementation of <see cref="IDistributedCacheManager.GetFromCacheAsync{T}(string, CancellationToken)"/>.
        /// </summary>
        public async Task<T> GetFromCacheAsync<T>(string key, CancellationToken cancellationToken)
        {
            var cachedMessage = await _distributedCache.GetStringAsync(key, cancellationToken);
            if (cachedMessage == null)
            {
                return default;
            }
            return (T)JsonSerializer.Deserialize(cachedMessage, typeof(T));
        }

        /// <summary>
        /// Implementation of <see cref="IDistributedCacheManager.GetFromCacheAsync(string, CancellationToken)"/>.
        /// </summary>
        public async Task<byte[]> GetFromCacheAsync(string key, CancellationToken cancellationToken)
        {
            return await _distributedCache.GetAsync(key, cancellationToken);
        }

        /// <summary>
        /// Implementation of <see cref="IDistributedCacheManager.Remove(string)"/>.
        /// </summary>
        public void Remove(string key)
        {
            _distributedCache.Remove(key);
        }

        /// <summary>
        /// Implementation of <see cref="IDistributedCacheManager.RemoveAsync(string, CancellationToken)"/>.
        /// </summary>
        public async Task RemoveAsync(string key, CancellationToken cancellationToken)
        {
            await _distributedCache.RemoveAsync(key, cancellationToken);
        }

        /// <summary>
        /// Implementation of <see cref="IDistributedCacheManager.SetCache{T}(string, T, CachingTypes)"/>.
        /// </summary>
        public void SetCache<T>(string key, T message, DistributedCacheEntryOptions options)
        {
            _distributedCache.SetString(key, JsonSerializer.Serialize(message, typeof(T)), options);
        }

        /// <summary>
        /// Implementation of <see cref="IDistributedCacheManager.SetCache(string, byte[], DistributedCacheEntryOptions)"/>.
        /// </summary>
        public void SetCache(string key, byte[] message, DistributedCacheEntryOptions options)
        {
            _distributedCache.Set(key, message, options);
        }

        /// <summary>
        /// Implementation of <see cref="IDistributedCacheManager.SetCacheAsync{T}(string, T, DistributedCacheEntryOptions, CancellationToken)"/>.
        /// </summary>
        public async Task SetCacheAsync<T>(string key, T message, DistributedCacheEntryOptions options, CancellationToken cancellationToken)
        {
            await _distributedCache.SetStringAsync(key, JsonSerializer.Serialize(message), options, cancellationToken);
        }

        /// <summary>
        /// Implementation of <see cref="IDistributedCacheManager.SetCacheAsync(string, byte[], DistributedCacheEntryOptions, CancellationToken)"/>.
        /// </summary>
        public async Task SetCacheAsync(string key, byte[] message, DistributedCacheEntryOptions options, CancellationToken cancellationToken)
        {
            await _distributedCache.SetAsync(key, message, options, cancellationToken);
        }
    }
}
