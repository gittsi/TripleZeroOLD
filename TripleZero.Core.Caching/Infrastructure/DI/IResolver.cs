using Autofac;
using System;
using System.Collections.Generic;
using System.Text;

namespace TripleZero.Core.Caching.Infrastructure.DI
{
    public class IResolver : ResolverConfig, IStartable
    {
        private static IResolver _Current;
        public static IResolver Current
        {
            get
            {
                return _Current;
            }
        }
        public IResolver()
        {

        }
        public void Start()
        {
            Container = ConfigureContainer();
            _Current = this;
        }
    }
}
