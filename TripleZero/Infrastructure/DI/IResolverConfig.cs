using Autofac;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using TripleZero._Mapping;
using TripleZero.Configuration;
using TripleZero.Modules;
using TripleZero.Repository.SWGoHRepository;

namespace TripleZero.Infrastructure.DI
{
    public abstract class ResolverConfig
    {
        internal IContainer Container { get; set; }

        public ApplicationSettings ApplicationSettings { get { return Container.Resolve<ApplicationSettings>(); } }
        public ISWGoHRepository SWGoHRepository  { get { return Container.Resolve<ISWGoHRepository>(); } }
        public IMappingConfiguration MappingConfiguration { get { return Container.Resolve<IMappingConfiguration>(); } }

        public static IContainer ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            //configurations
            builder.RegisterType<MappingConfiguration>().As<IMappingConfiguration>().SingleInstance();
            builder.RegisterType<ApplicationSettings>().SingleInstance();
            builder.RegisterType<SettingsConfiguration>().As<ISettingsConfiguration>().SingleInstance();

            builder.RegisterType<DiscordSocketClient>().SingleInstance();

            //repositories
            builder.RegisterType<SWGoHRepository>().As<ISWGoHRepository>().InstancePerDependency();

            return builder.Build();
        }
    }
}
