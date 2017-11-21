using System;
using System.Collections.Generic;
using System.Text;
using CacheManager.Core;
using TripleZero.Repository.Infrastructure.DI;

namespace TripleZero.Repository.Strategy
{
    internal class CachingRepositoryStrategy : CachingStrategy
    {
        ICacheManager<object> _cacheFactory = IResolver.Current.CachingFactory.GetFactoryRepository();

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
            int minutesBeforeExpiration = IResolver.Current.ApplicationSettings.GetTripleZeroRepositorySettings().CachingSettings.RepositoryCachingInMinutes;

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
