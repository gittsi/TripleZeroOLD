using Autofac;
using TripleZeroApi.Repository;

namespace TripleZeroApi.Infrastructure.DI
{
    public abstract class ResolverConfig
    {
        internal IContainer Container { get; set; }        
        public IMongoDBRepository MongoDBRepository { get { return Container.Resolve<IMongoDBRepository>(); } }



    public static IContainer ConfigureContainer()
    {
        var builder = new ContainerBuilder();

        //repositories
        builder.RegisterType<MongoDBRepository>().As<IMongoDBRepository>().InstancePerDependency();

        return builder.Build();
    }

    }
}
