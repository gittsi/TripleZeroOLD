using CacheManager.Core;
using System;

namespace TripleZero.Core.Caching
{
    public interface ICacheConfiguration
    {
        ICacheManagerConfiguration GetConfiguration();
    }
}
