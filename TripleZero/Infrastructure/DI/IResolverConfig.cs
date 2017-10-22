using Autofac;
using System;
using System.Collections.Generic;
using System.Text;
using TripleZero.Model;

namespace TripleZero.Infrastructure.DI
{
    public abstract class ResolverConfig
    {
        internal IContainer Container { get; set; }

        //public sddss sss { get { return Container.Resolve<ssss>(); } }
       
        public static IContainer ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            //configurations
            builder.RegisterType<TripleZeroBot>().As<TripleZeroBot>().SingleInstance();


            return builder.Build();
        }
    }
}
