using Autofac;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using TripleZero.Core.Caching;
using TripleZero.Core.Settings;
using TripleZero.Repository;
using TripleZero.Repository.Configuration;

namespace TripleZero.Infrastructure.DI
{
    public abstract class ResolverConfig
    {
        internal IContainer Container { get; set; }
        public ApplicationSettings ApplicationSettings { get { return Container.Resolve<ApplicationSettings>(); } }
        //public CachingFactory CachingFactory { get { return Container.Resolve<CachingFactory>(); } }
        //public MongoDBSettings MongoDBSettings { get { return Container.Resolve<MongoDBSettings>(); } }
        public GuildSettings GuildSettings { get { return Container.Resolve<GuildSettings>(); } }
        public CharacterSettings CharacterSettings { get { return Container.Resolve<CharacterSettings>(); } }

        public CacheClient CacheClient { get { return Container.Resolve<CacheClient>(); } }
        //public CharacterSettings CharacterSettings { get { return Container.Resolve<CharacterSettings>(); } }
        public ISWGoHRepository SWGoHRepository { get { return Container.Resolve<ISWGoHRepository>(); } }
        public IMongoDBRepository MongoDBRepository { get { return Container.Resolve<IMongoDBRepository>(); } }
        //public IMappingConfiguration MappingConfiguration { get { return Container.Resolve<IMappingConfiguration>(); } }

        //public CachingStrategyContext CachingStrategyContext { get { return Container.Resolve<CachingStrategyContext>(); } }
        //public CachingModuleStrategy CachingModuleStrategy { get { return Container.Resolve<CachingModuleStrategy>(); } }
        //public CachingRepositoryStrategy CachingRepositoryStrategy { get { return Container.Resolve<CachingRepositoryStrategy>(); } }
        public static IContainer ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            //configurations
            //builder.RegisterType<MappingConfiguration>().As<IMappingConfiguration>().SingleInstance();
            builder.RegisterType<ApplicationSettings>().SingleInstance();
            //builder.RegisterType<CachingFactory>().SingleInstance();
            //builder.RegisterType<MongoDBSettings>().SingleInstance();
            builder.RegisterType<GuildSettings>().SingleInstance();
            builder.RegisterType<CharacterSettings>().SingleInstance();
            //builder.RegisterType<CharacterSettings>().SingleInstance();
            builder.RegisterType<SettingsConfiguration>().As<ISettingsConfiguration>().SingleInstance();
            //builder.RegisterType<CacheConfiguration>().As<ICacheConfiguration>().SingleInstance();
            //builder.RegisterType<Caching>().As<ICaching>().SingleInstance();

            builder.RegisterType<DiscordSocketClient>().SingleInstance();

            //repositories
            builder.RegisterType<SWGoHRepository>().As<ISWGoHRepository>().InstancePerDependency();
            builder.RegisterType<MongoDBRepository>().As<IMongoDBRepository>().InstancePerDependency();

            //cachclient
            builder.RegisterType<CacheClient>().SingleInstance();

            //strategies
            //builder.RegisterType<CachingStrategy>().As<ICachingStrategy>().InstancePerDependency();
            //builder.RegisterType<CachingRepositoryStrategy>().SingleInstance();
            //builder.RegisterType<CachingModuleStrategy>().SingleInstance();

            //context            
            //builder.RegisterType<CachingStrategyContext>().InstancePerDependency();

            return builder.Build();
        }
    }
}
