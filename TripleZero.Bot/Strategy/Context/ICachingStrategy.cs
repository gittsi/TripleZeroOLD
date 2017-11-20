using CacheManager.Core;
using System;
using System.Collections.Generic;
using System.Text;
using TripleZero.Helper.Enum;

namespace TripleZero.Strategy
{
    public interface ICachingStrategy
    {
        //ICacheManager<object> GetCacheManager(EnumCacheStrategy enumCache);
        bool CacheAdd(string key, object obj, Int16 minutesBeforeExpiration);
        bool CacheAdd(string key, object obj);
        object CacheGetFromKey(string key);
    }
}
