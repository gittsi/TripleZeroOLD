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
        //internal SettingsTripleZeroRepository SettingsTripleZeroRepository { get { return Container.Resolve<SettingsTripleZeroRepository>(); } }
        //internal SettingsTripleZeroRepository SettingsTripleZeroRepository { get { return Container.Resolve<SettingsTripleZeroRepository>(); } }
        internal CachingFactory CachingFactory { get { return Container.Resolve<CachingFactory>(); } }
        //internal MongoDBSettings MongoDBSettings { get { return Container.Resolve<MongoDBSettings>(); } }
        //internal GuildSettings GuildSettings { get { return Container.Resolve<GuildSettings>(); } }
        //internal CharacterSettings CharacterSettings { get { return Container.Resolve<CharacterSettings>(); } }
        //internal ISWGoHRepository SWGoHRepository { get { return Container.Resolve<ISWGoHRepository>(); } }
        //internal IMongoDBRepository MongoDBRepository { get { return Container.Resolve<IMongoDBRepository>(); } }
        //public IMappingConfiguration MappingConfiguration { get { return Container.Resolve<IMappingConfiguration>(); } }

        internal CachingStrategyContext CachingStrategyContext { get { return Container.Resolve<CachingStrategyContext>(); } }
        internal CachingRepositoryStrategy CachingRepositoryStrategy { get { return Container.Resolve<CachingRepositoryStrategy>(); } }
        internal CachingModuleStrategy CachingModuleStrategy { get { return Container.Resolve<CachingModuleStrategy>(); } }
        internal static IContainer ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            //configurations
            //builder.RegisterType<MappingConfiguration>().As<IMappingConfiguration>().SingleInstance();
            builder.RegisterType<ApplicationSettings>().SingleInstance();
            builder.RegisterType<CachingFactory>().SingleInstance();
            builder.RegisterType<SettingsConfiguration>().As<ISettingsConfiguration>().SingleInstance();
            //builder.RegisterType<SettingsTripleZeroBot>().SingleInstance();
            //builder.RegisterType<SettingsTripleZeroRepository>().SingleInstance();
            //builder.RegisterType<CharacterSettings>().SingleInstance();
            //builder.RegisterType<CharacterSettings>().SingleInstance();
            //builder.RegisterType<SettingsConfiguration>().As<ISettingsConfiguration>().SingleInstance();
            builder.RegisterType<CacheConfiguration>().As<ICacheConfiguration>().SingleInstance();
            //builder.RegisterType<Caching>().As<ICaching>().SingleInstance();            

            //repositories
            //builder.RegisterType<SWGoHRepository>().As<ISWGoHRepository>().InstancePerDependency();
            //builder.RegisterType<MongoDBRepository>().As<IMongoDBRepository>().InstancePerDependency();

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

