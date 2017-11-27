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
        public GuildSettings GuildSettings { get { return Container.Resolve<GuildSettings>(); } }
        public CharacterSettings CharacterSettings { get { return Container.Resolve<CharacterSettings>(); } }
        public CacheClient CacheClient { get { return Container.Resolve<CacheClient>(); } }        
        public ISWGoHRepository SWGoHRepository { get { return Container.Resolve<ISWGoHRepository>(); } }
        public IMongoDBRepository MongoDBRepository { get { return Container.Resolve<IMongoDBRepository>(); } }        
        public static IContainer ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            //configurations            
            builder.RegisterType<ApplicationSettings>().SingleInstance();            
            builder.RegisterType<GuildSettings>().SingleInstance();
            builder.RegisterType<CharacterSettings>().SingleInstance();            
            builder.RegisterType<SettingsConfiguration>().As<ISettingsConfiguration>().SingleInstance();                        

            //repositories
            builder.RegisterType<SWGoHRepository>().As<ISWGoHRepository>().InstancePerDependency();
            builder.RegisterType<MongoDBRepository>().As<IMongoDBRepository>().InstancePerDependency();

            //cachclient
            builder.RegisterType<CacheClient>().SingleInstance();            

            return builder.Build();
        }
    }
}
