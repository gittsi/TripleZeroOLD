using Autofac.Extensions.DependencyInjection;
using Autofac;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace TripleZero.Infrastructure.DI
{
    public class IServiceProvider
    {

        public IContainer ApplicationContainer { get; private set; }
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // add other services, then
            var builder = new ContainerBuilder();

            // add other Autofac registrations, then
            IContainer container = null;
            services.Add(
              new ServiceDescriptor(
                typeof(IContainer),
                provider => container,
                ServiceLifetime.Singleton));

            builder.Populate(services);

            // yay for closures!
            container = builder.Build();
            return container.Resolve<IServiceProvider>();
        }

    }
}
