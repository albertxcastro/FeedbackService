using Microsoft.Extensions.Caching.Distributed;
using System.Threading;
using System.Threading.Tasks;

namespace CachingManager.Managers
{
    public interface IDistributedCacheManager
    {
        void SetCache<T>(string key, T message, DistributedCacheEntryOptions options);
        void SetCache(string key, byte[] message, DistributedCacheEntryOptions options);
        Task SetCacheAsync<T>(string key, T message, DistributedCacheEntryOptions options, CancellationToken cancellationToken);
        Task SetCacheAsync(string key, byte[] message, DistributedCacheEntryOptions options, CancellationToken cancellationToken);
        T GetFromCache<T>(string key);
        byte[] GetFromCache(string key);
        Task<T> GetFromCacheAsync<T>(string key, CancellationToken cancellationToken);
        Task<byte[]> GetFromCacheAsync(string key, CancellationToken cancellationToken);
    }
}
