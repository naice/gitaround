using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gitaround
{
    class Program
    {
        internal static Container Container;

        static void Main(string[] args)
        {
            Container = CreateIoCContainer(args);
            var program = Container.GetInstance<Provider.IProgram>();
            program.Run();
        }

        private static Container CreateIoCContainer(string[] args)
        {
            var container = new Container();
            // Top level
            container.Register<Model.ApplicationPath>(Lifestyle.Singleton);
            container.Register(() => new Model.CommandLineArgs(args), Lifestyle.Singleton);
            container.Register(() => Factories.ConfigurationFactory.Factory(container), Lifestyle.Singleton);
            container.Register<Provider.ILogger, Provider.Logger>(Lifestyle.Singleton);

            // Business
            container.RegisterCollection<Parseable.IParseable>(new Type[] {
                typeof(Parseable.SourceTree),
            });
            container.RegisterConditional<Provider.IProgram, Provider.CheckOutRefProvider>(Lifestyle.Transient, IProgramPerdicate);
            container.RegisterConditional<Provider.IProgram, Provider.UpdateRegistryProvider>(Lifestyle.Transient, IProgramPerdicate);
            container.RegisterConditional<Provider.IProgram, Provider.NoProgramFoundProvider>(Lifestyle.Transient, IProgramPerdicate);



#if DEBUG
            container.Verify();
#endif
            return container;
        }

        private static bool IProgramPerdicate(PredicateContext context)
        {
            var cmdArgs = Container.GetInstance<Model.CommandLineArgs>();

            if (cmdArgs.IsUpdateRegistry)
            {
                return context.ImplementationType == typeof(Provider.UpdateRegistryProvider);
            }

            if (!string.IsNullOrEmpty(cmdArgs.Url))
            {
                return context.ImplementationType == typeof(Provider.CheckOutRefProvider);
            }
            
            return context.ImplementationType == typeof(Provider.NoProgramFoundProvider);
        }
    }
}
