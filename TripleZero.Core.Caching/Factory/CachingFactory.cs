using CacheManager.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace TripleZero.Core.Caching
{
    public class CachingFactory
    {
        private readonly ICacheManagerConfiguration _cacheManagerConfiguration = null;
        public CachingFactory(ICacheConfiguration cachingConfiguration)
        {
            _cacheManagerConfiguration = cachingConfiguration.GetConfiguration();
        }
        public ICacheManager<object> GetFactoryRepository()
        {
            var cacheFactory = CacheFactory.Build("cacheRepository", settings => settings
            .WithDictionaryHandle("handleName")
            .EnableStatistics()
            .EnablePerformanceCounters());

            return cacheFactory;
        }

        public ICacheManager<object> GetFactoryModule()
        {
            var cacheFactory = CacheFactory.Build("cacheModule", settings => settings
            .WithDictionaryHandle("handleName")
            .EnableStatistics()
            .EnablePerformanceCounters());

            return cacheFactory;
        }
    }

}
