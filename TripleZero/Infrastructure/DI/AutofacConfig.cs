using Autofac;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using TripleZero.Configuration;
using TripleZero.Modules;

namespace TripleZero.Infrastructure.DI
{
    public static class AutofacConfig
    {
        public static IContainer ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<IResolver>().As<IStartable>().SingleInstance();

            builder.RegisterType<ApplicationSettings>().SingleInstance();
            builder.RegisterType<SettingsConfiguration>().As<ISettingsConfiguration>().SingleInstance();

            //modules
            builder.RegisterType<HelpModule>().InstancePerDependency();
            builder.RegisterType<MathModule>().InstancePerDependency();
            builder.RegisterType<TestModule>().InstancePerDependency();

            builder.RegisterType<DiscordSocketClient>().SingleInstance();

            builder.RegisterType<CommandService>().InstancePerDependency();

            return builder.Build();
        }

    }

}
