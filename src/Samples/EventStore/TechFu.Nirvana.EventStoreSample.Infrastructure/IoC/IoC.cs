using System.Reflection;
using StructureMap;
using StructureMap.Graph;
using TechFu.Nirvana.AzureQueues.Handlers;
using TechFu.Nirvana.CQRS.Queue;
using TechFu.Nirvana.EventStoreSample.Domain.Infrastructure;
using TechFu.Nirvana.EventStoreSample.Infrastructure.Io;
using TechFu.Nirvana.Mediation;
using TechFu.Nirvana.Mediation.Implementation;
using TechFu.Nirvana.Util;
using TechFu.Nirvana.Util.Io;
using TechFu.Nirvana.Util.Tine;
using TechFu.Nirvana.WebUtils;

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
        


                scan.AddAllTypesOf<IStartupStep>();
            });
            
            
            x.For<IMediator>().Use<Mediator>();
            x.For<IWebMediator>().Use<WebMediator>();
            x.For<IQueueFactory>().Use<AzureQueueFactory>();
            x.For<IAzureQueueConfiguration>().Use<AzureQueueConfiguration>();

            x.For<IQueueController>().Singleton().Use<AzureQueueController>();
            x.For<ISerializer>().Singleton().Use<Serializer>();
        }

        private static string GetStringForConnection(IContext c, ConnectionType connectionType)
        {
            return c.GetInstance<IConnectionStringService>().GetStringForConnection(connectionType);
        }
    }
}