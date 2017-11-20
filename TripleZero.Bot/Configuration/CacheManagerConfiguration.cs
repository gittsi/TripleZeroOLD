using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CacheManager.Core;

namespace TripleZero.Configuration
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
            //.EnableStatistics()
            ;
            
            Configuration = builder.Build();

            return Configuration;
        }

    }
}
