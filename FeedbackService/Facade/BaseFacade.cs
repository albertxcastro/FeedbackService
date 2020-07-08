using CachingManager.Managers;
using FeedbackService.Helpers;
using FeedbackService.Options;
using Microsoft.Extensions.Options;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FeedbackService.Facade
{
    public class BaseFacade
    {
        private readonly IDistributedCacheManager _distributedCacheManager;
        private readonly CacheOptions _cacheOptions;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public BaseFacade(IDistributedCacheManager distributedCacheManager, IOptions<CacheOptions> cacheOptions)
        {
            _distributedCacheManager = distributedCacheManager;
            _cacheOptions = cacheOptions.Value;
        }

        protected virtual async Task<List<T>> GetFromCacheAsync<T>(string typeName, string entityId, string exceptionMessage, CancellationToken cancellationToken)
        {
            List<T> cachedList = default;

            try
            {
                cachedList = await _distributedCacheManager.GetFromCacheAsync<List<T>>(string.Concat(_cacheOptions.ApplicationAlias, "_", entityId, "_", typeName), cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, exceptionMessage, entityId, _distributedCacheManager);
            }

            return cachedList;
        }

        protected virtual async Task SetCacheAsync<T>(List<T> entityList, string typeName, string entityId, CancellationToken cancellationToken)
        {
            try
            {
                var distributedCacheOptions = CacheHelper.GetCacheEntryOptions(typeName, _cacheOptions);
                await _distributedCacheManager.SetCacheAsync(string.Concat(_cacheOptions.ApplicationAlias, "_", entityId, "_", typeName), entityList, distributedCacheOptions, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, ex.Message, entityId, _distributedCacheManager);
            }
        }

        protected virtual async Task RemoveFromCacheAsync(string typeName, string entityId, CancellationToken cancellationToken)
        {
            try
            {
                await _distributedCacheManager.RemoveAsync(string.Concat(_cacheOptions.ApplicationAlias, "_", entityId, "_", typeName), cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, ex.Message, entityId, _distributedCacheManager);
            }
        }
    }
}
