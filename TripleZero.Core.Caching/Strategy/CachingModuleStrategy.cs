using System;
using System.Collections.Generic;
using System.Text;
using CacheManager.Core;
using TripleZero.Core.Caching.Infrastructure.DI;

namespace TripleZero.Core.Caching.Strategy
{
    public class CachingModuleStrategy : CachingStrategy
    {
        ICacheManager<object> _cacheFactory = IResolver.Current.CachingFactory.GetFactoryModule();

        public override bool CacheAdd(string key, object obj, short minutesBeforeExpiration)
        {
            bool isAdded = _cacheFactory.Add(key, obj);
            _cacheFactory.Expire(key, TimeSpan.FromMinutes(minutesBeforeExpiration));
            return isAdded;
        }
        public override bool CacheAdd(string key, object obj)
        {
            bool isAdded = _cacheFactory.Add(key, obj);
            if (!isAdded)
            {     
                return false;
            }
            int minutesBeforeExpiration = IResolver.Current.ApplicationSettings.GetTripleZeroBotSettings().CachingSettings.ModuleCachingInMinutes;

            _cacheFactory.Expire(key, TimeSpan.FromMinutes(minutesBeforeExpiration));            
            return isAdded;
        }
        public override object CacheGetFromKey(string key)
        {
            var ret = _cacheFactory.Get(key);
            
            return _cacheFactory.Get(key);
        }
    }
}
