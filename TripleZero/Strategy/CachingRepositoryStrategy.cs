using System;
using System.Collections.Generic;
using System.Text;
using CacheManager.Core;
using TripleZero.Infrastructure.DI;
using TripleZero.Helper;

namespace TripleZero.Strategy
{
    public class CachingRepositoryStrategy : CachingStrategy
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
            if(!isAdded)
            {
                Consoler.WriteLineInColor(string.Format("repository cache key : {0} not added!!!", key), ConsoleColor.Red);
                return false;
            }
            int minutesBeforeExpiration = IResolver.Current.ApplicationSettings.Get().CachingSettings.RepositoryCachingInMinutesApiKey;

            _cacheFactory.Expire(key, TimeSpan.FromMinutes(minutesBeforeExpiration));
            Consoler.WriteLineInColor(string.Format("added repository cache key : {0} for {1} minutes", key, minutesBeforeExpiration), ConsoleColor.Green);
            return isAdded;
        }

        public override object CacheGetFromKey(string key)
        {
            var ret = _cacheFactory.Get(key);
            if (ret != null)
                Consoler.WriteLineInColor(string.Format("found module cache key : {0} ", key), ConsoleColor.Green);

            return _cacheFactory.Get(key);
        }

        //public override ICacheManager<object> GetCacheManager()
        //{
        //    return IResolver.Current.CachingFactory.GetFactoryRepository();
        //}
    }
}
