using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gitaround.Provider
{
    internal class ServiceProvider
    {
        private readonly ILogger _logger;
        private readonly IEnumerable<Services.IService> _services;
        private readonly Model.CommandLineArgs _commandLineArgs;
        public ServiceProvider(ILogger logger, IEnumerable<Services.IService> services, Model.CommandLineArgs commandLineArgs)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _commandLineArgs = commandLineArgs ?? throw new ArgumentNullException(nameof(commandLineArgs));
        }

        internal void Launch()
        {
            Type concreteType = null;

            if (_commandLineArgs.IsUpdateRegistry)
            {
                concreteType = typeof(Services.UpdateRegistryService);
            }
            else if (!string.IsNullOrEmpty(_commandLineArgs.Url))
            {
                concreteType = typeof(Services.CheckOutRefService);
            }

            if (concreteType != null)
            {
                var service = _services.FirstOrDefault(iservice => iservice.GetType() == concreteType);

                if (service == null)
                {
                    throw new InvalidOperationException($"The requested service is not registered. {concreteType.FullName}");
                }

                service.Run();
            }


            _logger.Error(nameof(ServiceProvider), $"Missing command arguments. Exit.");
        }
    }
}
