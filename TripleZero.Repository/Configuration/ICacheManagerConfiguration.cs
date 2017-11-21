using System;
using System.Collections.Generic;
using System.Text;
using CacheManager.Core;

namespace TripleZero.Repository.Configuration
{
    internal interface ICacheConfiguration
    {
        ICacheManagerConfiguration GetConfiguration();
    }
}
