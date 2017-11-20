using System;
using System.Collections.Generic;
using System.Text;
using TripleZero.Repository.EnumDto;

namespace TripleZero.Repository.Strategy
{
    internal class CachingStrategyContext    
    {
        private CachingStrategy _CachingStrategy;

        public void SetStrategy(CachingStrategy cachingStrategy)
        {
            _CachingStrategy = cachingStrategy;            
        }
        public EnumCacheStrategy GetStrategy()
        {
            return this._CachingStrategy.Strategy();
        }
        public virtual bool CacheAdd(string key, object obj, short minutesBeforeExpiration)
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
    }
}
