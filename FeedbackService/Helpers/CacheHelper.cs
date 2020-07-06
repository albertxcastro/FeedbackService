using FeedbackService.Options;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Linq;

namespace FeedbackService.Helpers
{
    public static class CacheHelper
    {
        public static DistributedCacheEntryOptions GetCacheEntryOptions(string type, CacheOptions cacheOptions)
        {
            var cacheExpiry = cacheOptions.Expiry.Where(expiry => expiry.Key.ToLower().Equals(type.ToLower())).FirstOrDefault();
            if (cacheExpiry == null)
            {
                cacheExpiry = cacheOptions.Expiry.Where(expiry => expiry.Key.ToLower().Equals("default")).Single();
            }

            return new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(cacheExpiry.Value)
            };
        }
    }
}
