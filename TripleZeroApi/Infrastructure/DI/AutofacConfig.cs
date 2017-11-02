using Autofac;
using System;
using System.Collections.Generic;
using System.Text;
using TripleZeroApi.Repository;

namespace TripleZeroApi.Infrastructure.DI
{
    public static class AutofacConfig
    {
        public static IContainer ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<IResolver>().As<IStartable>().SingleInstance();
            //repositories
            builder.RegisterType<MongoDBRepository>().As<IMongoDBRepository>().InstancePerDependency();

            return builder.Build();
        }

    }

}
