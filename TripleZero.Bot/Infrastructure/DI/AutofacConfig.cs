using Autofac;
using Discord.Commands;
using Discord.WebSocket;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using TripleZero.Bot.Validators;
using TripleZero.Core.Settings;
//using TripleZero.Infrastructure.DI;
using TripleZero.Modules;
using TripleZero.Repository;

namespace TripleZero.Infrastructure.DI
{
    public static class AutofacConfig
    {
        public static IContainer ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            //Resolvers
            builder.RegisterType<TripleZero.Infrastructure.DI.IResolver>().As<IStartable>().SingleInstance();
            builder.RegisterType<TripleZero.Repository.Infrastructure.DI.IResolver>().As<IStartable>().SingleInstance();
            builder.RegisterType<TripleZero.Core.Caching.Infrastructure.DI.IResolver>().As<IStartable>().SingleInstance();
            
            //configuration
            builder.RegisterType<ApplicationSettings>().SingleInstance();                        
            builder.RegisterType<SettingsConfiguration>().As<ISettingsConfiguration>().SingleInstance();            

            //modules
            builder.RegisterType<HelpModule>().InstancePerDependency();
            builder.RegisterType<FunModule>().InstancePerDependency();
            builder.RegisterType<GuildModule>().InstancePerDependency();
            builder.RegisterType<CharacterModule>().InstancePerDependency();
            builder.RegisterType<ModsModule>().InstancePerDependency();
            builder.RegisterType<PlayerModule>().InstancePerDependency();
            builder.RegisterType<AdminModule>().InstancePerDependency();
            builder.RegisterType<DBStatsModule>().InstancePerDependency();

            //discord
            builder.RegisterType<DiscordSocketClient>().SingleInstance();

            //commandService
            builder.RegisterType<CommandService>().InstancePerDependency();

            //repositories
            builder.RegisterType<SWGoHRepository>().As<ISWGoHRepository>().InstancePerDependency();
            builder.RegisterType<MongoDBRepository>().As<IMongoDBRepository>().InstancePerDependency();

            //validator
            //builder.RegisterType<FluentValidationModelValidatorProvider>().As<ModelValidatorProvider>();
            builder.RegisterAssemblyTypes(System.Reflection.Assembly.GetExecutingAssembly())
                   .Where(t => t.Name.EndsWith("Validator"))
                   .AsImplementedInterfaces()
                   .InstancePerLifetimeScope();

            


            //builder.RegisterType<AutofacValidatorFactory2>().As<IValidatorFactory>().SingleInstance();
            builder.RegisterAssemblyTypes(System.Reflection.Assembly.GetExecutingAssembly())
            .Where(t => t.IsClosedTypeOf(typeof(IValidator<>)))
            .AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterType<PlayerValidator>().As<IValidator>().SingleInstance();

            //container.Register(typeof(IValidator<>), new[] { System.Reflection.Assembly.GetExecutingAssembly() }, Lifestyle.Singleton);
            //container.RegisterSingleton<IValidatorFactory>(() => new IocValidatorFactory(container));

            return builder.Build();
        }
    }

}
