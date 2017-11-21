using Autofac;
using System;
using System.Collections.Generic;
using System.Text;
using TripleZero.Core.Settings;
using TripleZero.Repository._Mapping;
using TripleZero.Repository.Configuration;
using TripleZero.Repository.Helper.Cache;
//using TripleZero.Helper.Cache;
//using TripleZero.Helper.Cache;
using TripleZero.Repository.Strategy;

namespace TripleZero.Repository.Infrastructure.DI
{
    public abstract class ResolverConfig
    {
        internal IContainer Container { get; set; }
        internal ApplicationSettings ApplicationSettings { get { return Container.Resolve<ApplicationSettings>(); } }
        internal CachingFactory CachingFactory { get { return Container.Resolve<CachingFactory>(); } }
        //internal MongoDBSettings MongoDBSettings { get { return Container.Resolve<MongoDBSettings>(); } }
        internal GuildSettings GuildSettings { get { return Container.Resolve<GuildSettings>(); } }
        internal CharacterSettings CharacterSettings { get { return Container.Resolve<CharacterSettings>(); } }
        internal ISWGoHRepository SWGoHRepository { get { return Container.Resolve<ISWGoHRepository>(); } }
        internal IMongoDBRepository MongoDBRepository { get { return Container.Resolve<IMongoDBRepository>(); } }
        public IMappingConfiguration MappingConfiguration { get { return Container.Resolve<IMappingConfiguration>(); } }

        internal CachingStrategyContext CachingStrategyContext { get { return Container.Resolve<CachingStrategyContext>(); } }
        internal CachingRepositoryStrategy CachingRepositoryStrategy { get { return Container.Resolve<CachingRepositoryStrategy>(); } }
        internal static IContainer ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            //configurations
            builder.RegisterType<MappingConfiguration>().As<IMappingConfiguration>().SingleInstance();
            builder.RegisterType<ApplicationSettings>().SingleInstance();
            builder.RegisterType<CachingFactory>().SingleInstance();
            //builder.RegisterType<MongoDBSettings>().SingleInstance();
            builder.RegisterType<GuildSettings>().SingleInstance();
            builder.RegisterType<CharacterSettings>().SingleInstance();
            //builder.RegisterType<CharacterSettings>().SingleInstance();
            builder.RegisterType<SettingsConfiguration>().As<ISettingsConfiguration>().SingleInstance();
            builder.RegisterType<CacheConfiguration>().As<ICacheConfiguration>().SingleInstance();
            //builder.RegisterType<Caching>().As<ICaching>().SingleInstance();            

            //repositories
            builder.RegisterType<SWGoHRepository>().As<ISWGoHRepository>().InstancePerDependency();
            builder.RegisterType<MongoDBRepository>().As<IMongoDBRepository>().InstancePerDependency();

            //strategies
            builder.RegisterType<CachingStrategy>().As<ICachingStrategy>().InstancePerDependency();
            builder.RegisterType<CachingRepositoryStrategy>().SingleInstance();            

            //context            
            builder.RegisterType<CachingStrategyContext>().InstancePerDependency();

            return builder.Build();
        }
    }
}
