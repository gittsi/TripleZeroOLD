using CacheManager.Core;
using System;

namespace TripleZero.Core.Caching
{
    internal interface ICacheConfiguration
    {
        ICacheManagerConfiguration GetConfiguration();
    }
}
