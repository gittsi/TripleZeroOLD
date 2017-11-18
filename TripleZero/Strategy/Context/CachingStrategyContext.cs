﻿using CacheManager.Core;
using System;
using System.Collections.Generic;
using System.Text;
using TripleZero.Helper.Enum;
using TripleZero.Infrastructure.DI;
//using TripleZero.Strategy.Decorator;
//using TripleZero.Strategy.Decorator;

namespace TripleZero.Strategy
{
    public class CachingStrategyContext    
    {
        private CachingStrategy _CachingStrategy;//= IResolver.Current.CachingStrategy;
        //private _DEL_CachingLogDecorator _CachingLogDecorator=  IResolver.Current.CachingLogDecorator;

        //public CachingStrategyContext(CachingLogDecorator cachingLogDecorator)
        //{
        //    _CachingLogDecorator = cachingLogDecorator;
        //}
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