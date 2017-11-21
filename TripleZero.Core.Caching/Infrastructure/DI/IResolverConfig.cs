using Autofac;
using System;
using System.Collections.Generic;
using System.Text;
using TripleZero.Core.Caching.Strategy;
using TripleZero.Core.Settings;

namespace TripleZero.Core.Caching.Infrastructure.DI
{
    public abstract class ResolverConfig
    {
        internal IContainer Container { get; set; }
        internal ApplicationSettings ApplicationSettings { get { return Container.Resolve<ApplicationSettings>(); } }
        internal SettingsConfiguration SettingsConfiguration { get { return Container.Resolve<SettingsConfiguration>(); } }        
        internal CachingFactory CachingFactory { get { return Container.Resolve<CachingFactory>(); } }
        internal CachingStrategyContext CachingStrategyContext { get { return Container.Resolve<CachingStrategyContext>(); } }
        internal CachingRepositoryStrategy CachingRepositoryStrategy { get { return Container.Resolve<CachingRepositoryStrategy>(); } }
        internal CachingModuleStrategy CachingModuleStrategy { get { return Container.Resolve<CachingModuleStrategy>(); } }
        internal static IContainer ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            //configurations
            builder.RegisterType<ApplicationSettings>().SingleInstance();
            builder.RegisterType<CachingFactory>().SingleInstance();
            builder.RegisterType<SettingsConfiguration>().As<ISettingsConfiguration>().SingleInstance();
            builder.RegisterType<CacheConfiguration>().As<ICacheConfiguration>().SingleInstance();

            //strategies
            builder.RegisterType<CachingStrategy>().As<ICachingStrategy>().InstancePerDependency();
            builder.RegisterType<CachingRepositoryStrategy>().SingleInstance();
            builder.RegisterType<CachingModuleStrategy>().SingleInstance();

            //context            
            builder.RegisterType<CachingStrategyContext>().InstancePerDependency();

            return builder.Build();
        }
    }
}

