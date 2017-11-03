using Autofac;
using Discord.WebSocket;
using SwGoh;
using System;
using System.Collections.Generic;
using System.Text;
using TripleZero._Mapping;
using TripleZero.Configuration;
using TripleZero.Modules;
using TripleZero.Repository;

namespace TripleZero.Infrastructure.DI
{
    public abstract class ResolverConfig
    {
        internal IContainer Container { get; set; }

        public ApplicationSettings ApplicationSettings { get { return Container.Resolve<ApplicationSettings>(); } }
        public MongoDBSettings MongoDBSettings { get { return Container.Resolve<MongoDBSettings>(); } }
        public Configuration.GuildsConfig GuildsConfig { get { return Container.Resolve<Configuration.GuildsConfig>(); } }
        //public CharacterSettings CharacterSettings { get { return Container.Resolve<CharacterSettings>(); } }
        public ISWGoHRepository SWGoHRepository  { get { return Container.Resolve<ISWGoHRepository>(); } }
        public IMongoDBRepository MongoDBRepository { get { return Container.Resolve<IMongoDBRepository>(); } }
        public IMappingConfiguration MappingConfiguration { get { return Container.Resolve<IMappingConfiguration>(); } }

        public static IContainer ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            //configurations
            builder.RegisterType<MappingConfiguration>().As<IMappingConfiguration>().SingleInstance();
            builder.RegisterType<ApplicationSettings>().SingleInstance();
            builder.RegisterType<MongoDBSettings>().SingleInstance();
            builder.RegisterType<Configuration.GuildsConfig>().SingleInstance();
            //builder.RegisterType<CharacterSettings>().SingleInstance();
            builder.RegisterType<SettingsConfiguration>().As<ISettingsConfiguration>().SingleInstance();

            builder.RegisterType<DiscordSocketClient>().SingleInstance();

            //repositories
            builder.RegisterType<SWGoHRepository>().As<ISWGoHRepository>().InstancePerDependency();
            builder.RegisterType<MongoDBRepository>().As<IMongoDBRepository>().InstancePerDependency();

            return builder.Build();
        }
    }
}
