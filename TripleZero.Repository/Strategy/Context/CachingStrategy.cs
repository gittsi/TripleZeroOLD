﻿using System;
using System.Collections.Generic;
using System.Text;
using TripleZero.Repository.EnumDto;

namespace TripleZero.Repository.Strategy
{
    internal abstract class CachingStrategy : ICachingStrategy
    {
        public abstract bool CacheAdd(string key, object obj, short minutesBeforeExpiration);
        public abstract bool CacheAdd(string key, object obj);

        public abstract object CacheGetFromKey(string key);        

        //public abstract ICacheManager<object> GetCacheManager();
        public virtual EnumCacheStrategy Strategy() { return  EnumCacheStrategy.Unknown; }
    }
}