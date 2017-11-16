using CacheManager.Core;
using System;
using System.Collections.Generic;
using System.Text;
using TripleZero.Helper.Enum;

namespace TripleZero.Strategy
{
    public class CachingStrategyContext    
    {
        private CachingStrategy _CachingStrategy;
        public void SetStrategy(CachingStrategy cachingStrategy)
        {
            this._CachingStrategy = cachingStrategy;
        }
        public EnumCacheStrategy GetStrategy()
        {
            return this._CachingStrategy.Strategy();
        }
        public bool CacheAdd(string key, object obj, short minutesBeforeExpiration)
        {
            if (_CachingStrategy is null)
            {
                throw new Exception("You have to choose strategy");
            }
            return this._CachingStrategy.CacheAdd(key,obj,minutesBeforeExpiration);
        }

        public bool CacheAdd(string key, object obj)
        {
            if (_CachingStrategy is null)
            {
                throw new Exception("You have to choose strategy");
            }
            return this._CachingStrategy.CacheAdd(key, obj);
        }

        public object CacheGetFromKey(string key)
        {
            if (_CachingStrategy is null)
            {
                throw new Exception("You have to choose strategy");
            }
            return this._CachingStrategy.CacheGetFromKey(key);
        }

        //public ICacheManager<object> GetCacheManager()
        //{
        //    if (_CachingStrategy is null)
        //    {
        //        throw new Exception("You have to choose strategy");
        //    }

        //    return this._CachingStrategy.GetCacheManager();
        //}
    }
}
