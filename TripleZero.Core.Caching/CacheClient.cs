using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TripleZero.Core.Caching.Infrastructure.DI;
using TripleZero.Core.Caching.Strategy;

namespace TripleZero.Core.Caching
{
    public class CacheClient
    {
        public string GetCachedDataRepositoryMessage() { return "\n**(cached data^^)**";  }

        public string GetMessageFromModuleCache(string functionName, string key)
        {
            CachingStrategyContext _CachingStrategyContext = IResolver.Current.CachingStrategyContext;
            CachingModuleStrategy _CachingModuleStrategy = IResolver.Current.CachingModuleStrategy;
            _CachingStrategyContext.SetStrategy(_CachingModuleStrategy);

            string retStr = "";
            string loadingStr = "";

            string strCacheKey = string.Concat(functionName, "-", key);
            var objCache = _CachingStrategyContext.CacheGetFromKey(strCacheKey);
            if (objCache != null)
            {
                //loadingStr = "\nCommand is loaded from module cache\n\n";
                //loadingStr = "\n---------------------------------------------";
                loadingStr = "\n**(cached data^)**";
                retStr = (string)objCache;                
                return string.Concat(loadingStr,retStr);
            }
            return string.Empty;
        }

        //public static object MessageFromRepositoryCache(string functionName, string key)
        //{
        //    CachingStrategyContext _CachingStrategyContext = IResolver.Current.CachingStrategyContext;
        //    CachingRepositoryStrategy _CachingRepositoryStrategy = IResolver.Current.CachingRepositoryStrategy;
        //    _CachingStrategyContext.SetStrategy(_CachingRepositoryStrategy);

        //    string strCacheKey = string.Concat(functionName, "-", key);
        //    var objCache = _CachingStrategyContext.CacheGetFromKey(strCacheKey);            
        //    return objCache;            
        //}

        public async Task<bool> AddToModuleCache(string functionName, string key, string retStr)
        {
            await Task.FromResult(1);

            CachingStrategyContext _CachingStrategyContext = IResolver.Current.CachingStrategyContext;
            CachingModuleStrategy _CachingModuleStrategy = IResolver.Current.CachingModuleStrategy;
            _CachingStrategyContext.SetStrategy(_CachingModuleStrategy);

            return await AddToCache(_CachingStrategyContext, functionName, key, retStr);
        }

        //public static async Task<bool> AddToRepositoryCache(string functionName, string key, object obj)
        //{
        //    await Task.FromResult(1);

        //    CachingStrategyContext _CachingStrategyContext = IResolver.Current.CachingStrategyContext;
        //    CachingRepositoryStrategy _CachingRepositoryStrategy = IResolver.Current.CachingRepositoryStrategy;
        //    _CachingStrategyContext.SetStrategy(_CachingRepositoryStrategy);

        //    return await AddToCache(_CachingStrategyContext, functionName, key, obj);
        //}

        //public static async Task<bool> AddToRepositoryCache(string functionName, string key, object obj, short minutesBeforeExpiration)
        //{
        //    await Task.FromResult(1);

        //    CachingStrategyContext _CachingStrategyContext = IResolver.Current.CachingStrategyContext;
        //    CachingRepositoryStrategy _CachingRepositoryStrategy = IResolver.Current.CachingRepositoryStrategy;
        //    _CachingStrategyContext.SetStrategy(_CachingRepositoryStrategy);

        //    return await AddToCache(_CachingStrategyContext, functionName, key, obj, minutesBeforeExpiration);
        //}

        public object GetDataFromRepositoryCache(string functionName, string key)
        {
            CachingStrategyContext _CachingStrategyContext = IResolver.Current.CachingStrategyContext;
            CachingRepositoryStrategy _CachingRepositoryStrategy = IResolver.Current.CachingRepositoryStrategy;
            _CachingStrategyContext.SetStrategy(_CachingRepositoryStrategy);

            string strCacheKey = string.Concat(functionName, "-", key);
            var objCache = _CachingStrategyContext.CacheGetFromKey(strCacheKey);
            return objCache;
        }

        public async Task<bool> AddToRepositoryCache(string functionName, string key, object obj)
        {
            await Task.FromResult(1);

            CachingStrategyContext _CachingStrategyContext = IResolver.Current.CachingStrategyContext;
            CachingRepositoryStrategy _CachingRepositoryStrategy = IResolver.Current.CachingRepositoryStrategy;
            _CachingStrategyContext.SetStrategy(_CachingRepositoryStrategy);

            return await AddToCache(_CachingStrategyContext, functionName, key, obj);
        }

        public async Task<bool> AddToRepositoryCache(string functionName, string key, object obj, short minutesBeforeExpiration)
        {
            await Task.FromResult(1);

            CachingStrategyContext _CachingStrategyContext = IResolver.Current.CachingStrategyContext;
            CachingRepositoryStrategy _CachingRepositoryStrategy = IResolver.Current.CachingRepositoryStrategy;
            _CachingStrategyContext.SetStrategy(_CachingRepositoryStrategy);

            return await AddToCache(_CachingStrategyContext, functionName, key, obj, minutesBeforeExpiration);
        }


        private async Task<bool> AddToCache(CachingStrategyContext cachingStrategyContext, string functionName, string key, object retStr)
        {
            await Task.FromResult(1);

            string strCacheKey = string.Concat(functionName, "-", key);
            return cachingStrategyContext.CacheAdd(strCacheKey, retStr);
        }

        private async Task<bool> AddToCache(CachingStrategyContext cachingStrategyContext, string functionName, string key, object retStr, short minutesBeforeExpiration)
        {
            await Task.FromResult(1);

            string strCacheKey = string.Concat(functionName, "-", key);
            return cachingStrategyContext.CacheAdd(strCacheKey, retStr, minutesBeforeExpiration);
        }
    }
}
