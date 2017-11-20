using System;
using System.Collections.Generic;
using System.Text;
using CacheManager.Core;

namespace TripleZero.Configuration
{
    public interface ICacheConfiguration
    {
        ICacheManagerConfiguration GetConfiguration();
    }
}
