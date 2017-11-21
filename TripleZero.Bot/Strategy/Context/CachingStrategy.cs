using System;
using System.Collections.Generic;
using System.Text;
using CacheManager.Core;
using TripleZero.Helper.Enum;

namespace TripleZero.Strategy
{
    public abstract class CachingStrategy : ICachingStrategy
    {
        public abstract bool CacheAdd(string key, object obj, short minutesBeforeExpiration);
        public abstract bool CacheAdd(string key, object obj);

        public abstract object CacheGetFromKey(string key);        

        //public abstract ICacheManager<object> GetCacheManager();
        public virtual EnumCacheStrategy Strategy() { return  EnumCacheStrategy.Unknown; }
    }
}
