using System;
using System.Collections.Generic;
using System.Text;
using TripleZero.Core.Caching.Enum;

namespace TripleZero.Core.Caching.Strategy
{
    internal class CachingStrategyContext    
    {
        internal CachingStrategy _CachingStrategy;//= IResolver.Current.CachingStrategy;
        //private _DEL_CachingLogDecorator _CachingLogDecorator=  IResolver.Current.CachingLogDecorator;

        //public CachingStrategyContext(CachingLogDecorator cachingLogDecorator)
        //{
        //    _CachingLogDecorator = cachingLogDecorator;
        //}
        internal void SetStrategy(CachingStrategy cachingStrategy)
        {
            _CachingStrategy = cachingStrategy;            
        }
        internal EnumCacheStrategy GetStrategy()
        {
            return this._CachingStrategy.Strategy();
        }
        public virtual bool CacheAdd(string key, object obj, short minutesBeforeExpiration)
        {
            if (_CachingStrategy is null)
            {
                throw new Exception("You have to choose strategy");
            }
            //return this._CachingLogDecorator.CacheAdd(key, obj, minutesBeforeExpiration);
            return this._CachingStrategy.CacheAdd(key,obj,minutesBeforeExpiration);
        }

        public bool CacheAdd(string key, object obj)
        {
            if (_CachingStrategy is null)
            {
                throw new Exception("You have to choose strategy");
            }
            //return this._CachingLogDecorator.CacheAdd(key, obj);
            return this._CachingStrategy.CacheAdd(key, obj);
        }

        public object CacheGetFromKey(string key)
        {
            if (_CachingStrategy is null)
            {
                throw new Exception("You have to choose strategy");
            }
            //return this._CachingLogDecorator.CacheGetFromKey(key);
            return this._CachingStrategy.CacheGetFromKey(key);
        }
    }
}
