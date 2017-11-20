using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TripleZero.Repository.Infrastructure.DI;
using TripleZero.Repository.Strategy;

namespace TripleZero.Repository.Helper.Cache
{
    internal static class CacheClient
    {
        public static string CachedDataRepository() { return "\n(cached data^^)";  }        

        public static object GetDataFromRepositoryCache(string functionName, string key)
        {
            CachingStrategyContext _CachingStrategyContext = IResolver.Current.CachingStrategyContext;
            CachingRepositoryStrategy _CachingRepositoryStrategy = IResolver.Current.CachingRepositoryStrategy;
            _CachingStrategyContext.SetStrategy(_CachingRepositoryStrategy);

            string strCacheKey = string.Concat(functionName, "-", key);
            var objCache = _CachingStrategyContext.CacheGetFromKey(strCacheKey);            
            return objCache;            
        }        

        public static async Task<bool> AddToRepositoryCache(string functionName, string key, object obj)
        {
            await Task.FromResult(1);

            CachingStrategyContext _CachingStrategyContext = IResolver.Current.CachingStrategyContext;
            CachingRepositoryStrategy _CachingRepositoryStrategy = IResolver.Current.CachingRepositoryStrategy;
            _CachingStrategyContext.SetStrategy(_CachingRepositoryStrategy);

            return await AddToCache(_CachingStrategyContext, functionName, key, obj);
        }

        public static async Task<bool> AddToRepositoryCache(string functionName, string key, object obj, short minutesBeforeExpiration)
        {
            await Task.FromResult(1);

            CachingStrategyContext _CachingStrategyContext = IResolver.Current.CachingStrategyContext;
            CachingRepositoryStrategy _CachingRepositoryStrategy = IResolver.Current.CachingRepositoryStrategy;
            _CachingStrategyContext.SetStrategy(_CachingRepositoryStrategy);

            return await AddToCache(_CachingStrategyContext, functionName, key, obj, minutesBeforeExpiration);
        }

        private static async Task<bool> AddToCache(CachingStrategyContext cachingStrategyContext, string functionName, string key, object retStr)
        {
            await Task.FromResult(1);

            string strCacheKey = string.Concat(functionName, "-", key);
            return cachingStrategyContext.CacheAdd(strCacheKey, retStr);
        }

        private static async Task<bool> AddToCache(CachingStrategyContext cachingStrategyContext, string functionName, string key, object retStr, short minutesBeforeExpiration)
        {
            await Task.FromResult(1);

            string strCacheKey = string.Concat(functionName, "-", key);
            return cachingStrategyContext.CacheAdd(strCacheKey, retStr, minutesBeforeExpiration);
        }
    }
}
