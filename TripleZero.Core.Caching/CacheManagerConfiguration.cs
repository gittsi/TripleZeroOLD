using CacheManager.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace TripleZero.Core.Caching
{
    public class CacheConfiguration : ICacheConfiguration
    {
        public static ICacheManagerConfiguration Configuration { get; set; }
        public CacheConfiguration()
        {
            Configuration = GetConfiguration();
        }
        public ICacheManagerConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
            .WithDictionaryHandle()            
            ;

            Configuration = builder.Build();

            return Configuration;
        }

    }
}
