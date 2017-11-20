using Autofac;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using TripleZero._Mapping;
using TripleZero.Configuration;
using TripleZero.Modules;
using TripleZero.Repository;
using TripleZero.Strategy;

namespace TripleZero.Repository.Infrastructure.DI
{
    public static class AutofacConfig
    {
        public static IContainer ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<IResolver>().As<IStartable>().SingleInstance();

            builder.RegisterType<MappingConfiguration>().As<IMappingConfiguration>().SingleInstance();
            builder.RegisterType<ApplicationSettings>().SingleInstance();            
            //builder.RegisterType<CachingFactory>().SingleInstance();
            builder.RegisterType<MongoDBSettings>().SingleInstance();
            builder.RegisterType<GuildSettings>().SingleInstance();
            builder.RegisterType<CharacterSettings>().SingleInstance();
            //builder.RegisterType<CharacterSettings>().SingleInstance();
            builder.RegisterType<SettingsConfiguration>().As<ISettingsConfiguration>().SingleInstance();
            builder.RegisterType<CacheConfiguration>().As<ICacheConfiguration>().SingleInstance();
            //builder.RegisterType<Caching>().As<ICaching>().SingleInstance();

            //modules
            builder.RegisterType<HelpModule>().InstancePerDependency();
            builder.RegisterType<FunModule>().InstancePerDependency();
            builder.RegisterType<GuildModule>().InstancePerDependency();
            builder.RegisterType<CharacterModule>().InstancePerDependency();
            builder.RegisterType<ModsModule>().InstancePerDependency();
            builder.RegisterType<PlayerModule>().InstancePerDependency();
            builder.RegisterType<AdminModule>().InstancePerDependency();
            builder.RegisterType<DBStatsModule>().InstancePerDependency();

            builder.RegisterType<DiscordSocketClient>().SingleInstance();

            builder.RegisterType<CommandService>().InstancePerDependency();

            //repositories
            builder.RegisterType<SWGoHRepository>().As<ISWGoHRepository>().InstancePerDependency();
            builder.RegisterType<MongoDBRepository>().As<IMongoDBRepository>().InstancePerDependency();

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
