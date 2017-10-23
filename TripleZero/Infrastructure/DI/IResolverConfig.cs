using Autofac;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using TripleZero.Configuration;
using TripleZero.Modules;

namespace TripleZero.Infrastructure.DI
{
    public abstract class ResolverConfig
    {
        internal IContainer Container { get; set; }

        public ApplicationSettings applicationSettings { get { return Container.Resolve<ApplicationSettings>(); } }

        public static IContainer ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            //configurations
            builder.RegisterType<ApplicationSettings>().SingleInstance();
            builder.RegisterType<SettingsConfiguration>().As<ISettingsConfiguration>().SingleInstance();

            builder.RegisterType<DiscordSocketClient>().SingleInstance();

            //modules
            builder.RegisterType<HelpModule>().InstancePerDependency();
            builder.RegisterType<MathModule>().InstancePerDependency();
            builder.RegisterType<TestModule>().InstancePerDependency();

            return builder.Build();
        }
    }
}
