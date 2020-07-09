using Microsoft.Extensions.Caching.Distributed;
using System.Threading;
using System.Threading.Tasks;

namespace CachingManager.Managers
{
    /// <summary>
    /// Used to set or retrieve data from the distributed cache
    /// </summary>
    public interface IDistributedCacheManager
    {
        /// <summary>
        /// Sets an object of the specified type in the cache.
        /// </summary>
        /// <typeparam name="T">The type of the object to be cached. For use with <see cref="System.Byte"/>[] use <see cref="SetCache(string, byte[], DistributedCacheEntryOptions)"/>instead.</typeparam>
        /// <param name="key">The key to store the data in.</param>
        /// <param name="message">The data to store in the cache.</param>
        /// <param name="options"><see cref="DistributedCacheEntryOptions"/> for the object to be chached</param>
        /// <exception cref="ArgumentNullException"></exception>
        void SetCache<T>(string key, T message, DistributedCacheEntryOptions options);

        /// <summary>
        /// Sets an object in the cache.
        /// </summary>
        /// <param name="key">The key to store the data in.</param>
        /// <param name="message">The data to store in the cache.</param>
        /// <param name="options"><see cref="DistributedCacheEntryOptions"/> for the object to be chached</param>
        /// <exception cref="ArgumentNullException"></exception>
        void SetCache(string key, byte[] message, DistributedCacheEntryOptions options);

        /// <summary>
        /// Asynchronously sets an object of the specified type in the cache.
        /// </summary>
        /// <typeparam name="T">The type of the object to be cached. For use with <see cref="System.Byte"/>[] use <see cref="SetCacheAsync(string, byte[], DistributedCacheEntryOptions, CancellationToken)"/> instead.</typeparam>
        /// <param name="key">The key to store the data in.</param>
        /// <param name="message">The data to store in the cache.</param>
        /// <param name="options"><see cref="DistributedCacheEntryOptions"/> for the object to be chached</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        Task SetCacheAsync<T>(string key, T message, DistributedCacheEntryOptions options, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously sets an object in the cache.
        /// </summary>
        /// <param name="key">The key to store the data in.</param>
        /// <param name="message">The data to store in the cache.</param>
        /// <param name="options"><see cref="DistributedCacheEntryOptions"/> for the object to be chached</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        Task SetCacheAsync(string key, byte[] message, DistributedCacheEntryOptions options, CancellationToken cancellationToken);

        /// <summary>
        /// Gets an object of the specified type from the cache with the specified key.
        /// </summary>
        /// <typeparam name="T">The type of the object to be retrieved. For use with <see cref="System.Byte"/>[] use <see cref="GetFromCache(string)"/>.</typeparam>
        /// <param name="key">The key to get the stored data for.</param>
        /// <returns>Object containing the result from cache. Otherwise default value of <see cref="{T}"/>.</returns>
        T GetFromCache<T>(string key);

        /// <summary>
        /// Gets an object from the cache with the specified key.
        /// </summary>
        /// <param name="key">The key to get the stored data for.</param>
        /// <returns>Object containing the result from cache.</returns>s
        byte[] GetFromCache(string key);

        /// <summary>
        /// Asynchronously gets an object of the specified type from the cache with the specified key.
        /// </summary>
        /// <typeparam name="T">The type of the object to be retrieved. For use with <see cref="System.Byte"/>[] use <see cref="GetFromCacheAsync(string, CancellationToken)"/></typeparam>
        /// <param name="key">The key to get the stored data for.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the located value or null.</returns>
        Task<T> GetFromCacheAsync<T>(string key, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously gets an object from the cache with the specified key.
        /// </summary>
        /// <param name="key">The key to get the stored data for.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the located value or null.</returns>
        Task<byte[]> GetFromCacheAsync(string key, CancellationToken cancellationToken);

        /// <summary>
        /// Removes an object from the cache with the specified key.
        /// </summary>
        /// <param name="key">The key to get the stored data for.</param>
        void Remove(string key);

        /// <summary>
        /// Asynchronously removes an object from the cache with the specified key.
        /// </summary>
        /// <param name="key">The key to get the stored data for.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the located value or null.</returns>
        Task RemoveAsync(string key, CancellationToken cancellationToken);
    }
}
