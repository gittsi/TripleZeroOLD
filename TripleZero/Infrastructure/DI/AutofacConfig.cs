using Autofac;
using System;
using System.Collections.Generic;
using System.Text;
using TripleZero.Model;

namespace TripleZero.Infrastructure.DI
{
    public static class AutofacConfig
    {
        public static IContainer ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<IResolver>().As<IStartable>().SingleInstance();

            builder.RegisterType<TripleZeroBot>().As<TripleZeroBot>().SingleInstance();

            // add other registrations here...

            return builder.Build();
        }
    }
}
