using System.Data.Entity;
using System.Reflection;
using StructureMap;
using StructureMap.Graph;
using TechFu.Nirvana.AzureQueues.Handlers;
using TechFu.Nirvana.Configuration;
using TechFu.Nirvana.CQRS.Queue;
using TechFu.Nirvana.Data;
using TechFu.Nirvana.EventStoreSample.Domain.Infrastructure;
using TechFu.Nirvana.EventStoreSample.Infrastructure.Io;
using TechFu.Nirvana.Mediation;
using TechFu.Nirvana.Mediation.Implementation;
using TechFu.Nirvana.MongoProvider;
using TechFu.Nirvana.SqlProvider;
using TechFu.Nirvana.Util;
using TechFu.Nirvana.Util.Io;
using TechFu.Nirvana.Util.Tine;
using TechFu.Nirvana.WebUtils;
using RdbmsContext = TechFu.Nirvana.EventStoreSample.Infrastructure.Domain.RdbmsContext;

namespace TechFu.Nirvana.EventStoreSample.Infrastructure.IoC
{
    public static class IoC
    {
        public static IContainer Initialize(Assembly assembly = null)
        {
            var container = new Container(x => { DoIoCInit(x, assembly); });
            InternalDependencyResolver.SetRootContainer(container);
            DoStartup(container);
            return container;
        }

        public static void DoStartup(IContainer container)
        {
            container.GetInstance<Startup>().Start();
        }

        public static void DoIoCInit(ConfigurationExpression x, Assembly assembly = null)
        {
            x.Scan(scan =>
            {
                scan.TheCallingAssembly();
                scan.AssemblyContainingType<IApplicationConfiguration>();
              
                if (assembly != null) scan.Assembly(assembly);

                scan.WithDefaultConventions();
                scan.AssemblyContainingType<ISystemTime>();

                scan.ConnectImplementationsToTypesClosing(typeof(ICommandHandler<,>));
                scan.ConnectImplementationsToTypesClosing(typeof(IQueryHandler<,>));
                scan.ConnectImplementationsToTypesClosing(typeof(IEventHandler<>));



                scan.AddAllTypesOf<IStartupStep>();
            });
            
            
//            x.For<IMediatorFactory>().Use<MediatorFactory>();
//            x.For<IChildMediatorFactory>().Use<ChildMediatorFactory>();
            x.For<IMediator>().Use<Mediator>();
            x.For<IWebMediator>().Use<WebMediator>();
            x.For<IQueueFactory>().Use<AzureQueueFactory>();
            x.For<IAzureQueueConfiguration>().Use<AzureQueueConfiguration>();
            x.For<INirvanaConfiguration>().Use<NirvanaConfiguration>();


            //Data Providers
            x.For<IRepository<object>>().Use<SqlRepository<object>>();
            x.For<IViewModelRepository>().Use<MonogoViewModelRepository>();
            x.For<NirvanaMongoConfiguration>().Use(c=> new NirvanaMongoConfiguration
            {
                Database = "viewModels",
                Password = "password",
                ServerName = "localhost",
                UserName = "viewModelUser",
                Port=27017
                // = "mongodb://viewModeluser:password@127.0.0.1:24017/local"
            });

            //TODO - plug in the connection string DR provider
            x.For<DbContext>().Use(d=>new RdbmsContext(SaveChangesDecoratorType.Live, "DataStoreConnectionString"));


            x.For<IQueueController>().Singleton().Use<AzureQueueController>();
            x.For<ISerializer>().Singleton().Use<Serializer>();
            x.For<INirvanaHttpClient>().Singleton().Use<NirvanaHttpClient>();



        }

        private static string GetStringForConnection(IContext c, ConnectionType connectionType)
        {
            return c.GetInstance<IConnectionStringService>().GetStringForConnection(connectionType);
        }
    }
}