using SimpleInjector;
using SimpleInjector.Lifestyles;
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
            var serviceProvider = Container.GetInstance<Provider.ServiceProvider>();
            serviceProvider.Launch();
        }

        private static Container CreateIoCContainer(string[] args)
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new ThreadScopedLifestyle();

            // Iteration 0
            container.Register<Model.ApplicationPath>(Lifestyle.Singleton);
            container.Register(() => new Model.CommandLineArgs(args), Lifestyle.Singleton);
            container.Register(() => Factories.ConfigurationFactory.Factory(container), Lifestyle.Singleton);
            container.Register<Provider.ILogger, Provider.Logger>(Lifestyle.Singleton);

            // Adapter
            container.Register<GitAdapter.IGitAdapter, GitAdapter.LibGit2SharpGitAdapter>();

            // Business
            container.RegisterCollection<Parseable.IParseable>(new Type[] {
                typeof(Parseable.SourceTree),
            });
            container.RegisterCollection<Services.IService>(new Type[] {
                typeof(Services.CheckOutRefService),
                typeof(Services.UpdateRegistryService),
            });

#if DEBUG
            container.Verify();
#endif
            return container;
        }

    }
}
