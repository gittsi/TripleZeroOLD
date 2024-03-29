﻿using System;
using System.Collections.Generic;
using System.Text;


namespace TripleZero.Core.Caching.Strategy
{
    internal interface ICachingStrategy
    {        
        bool CacheAdd(string key, object obj, Int16 minutesBeforeExpiration);
        bool CacheAdd(string key, object obj);
        void ClearCache();
        object CacheGetFromKey(string key);
    }
}
