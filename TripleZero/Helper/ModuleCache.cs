using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TripleZero.Infrastructure.DI;
using TripleZero.Strategy;

namespace TripleZero.Helper
{
    public static class ModuleCache
    {
        public static string MessageFromCache(string functionName, string key)
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
                loadingStr = "\n---------------------------------------------";
                retStr = (string)objCache;                
                return string.Concat(loadingStr,retStr);
            }
            return string.Empty;
        }

        public static async Task<bool> AddToCache(string functionName, string key, string retStr)
        {
            await Task.FromResult(1);

            CachingStrategyContext _CachingStrategyContext = IResolver.Current.CachingStrategyContext;
            CachingModuleStrategy _CachingModuleStrategy = IResolver.Current.CachingModuleStrategy;
            _CachingStrategyContext.SetStrategy(_CachingModuleStrategy);

            string strCacheKey = string.Concat(functionName, "-", key);
            return _CachingStrategyContext.CacheAdd(strCacheKey, retStr);
        }
    }
}
