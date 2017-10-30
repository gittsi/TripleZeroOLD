using Autofac;
using Discord.Commands;
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
    public static class AutofacConfig
    {
        public static IContainer ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<IResolver>().As<IStartable>().SingleInstance();

            builder.RegisterType<MappingConfiguration>().As<IMappingConfiguration>().SingleInstance();
            builder.RegisterType<ApplicationSettings>().SingleInstance();
            builder.RegisterType<GuildSettings>().SingleInstance();
            builder.RegisterType<CharacterSettings>().SingleInstance();
            builder.RegisterType<SettingsConfiguration>().As<ISettingsConfiguration>().SingleInstance();

            //modules
            builder.RegisterType<HelpModule>().InstancePerDependency();
            builder.RegisterType<MathModule>().InstancePerDependency();
            builder.RegisterType<FunModule>().InstancePerDependency();
            builder.RegisterType<GuildModule>().InstancePerDependency();
            builder.RegisterType<CharacterModule>().InstancePerDependency();

            builder.RegisterType<DiscordSocketClient>().SingleInstance();

            builder.RegisterType<CommandService>().InstancePerDependency();

            //repositories
            builder.RegisterType<SWGoHRepository>().As<ISWGoHRepository>().InstancePerDependency();

            return builder.Build();
        }

    }

}
