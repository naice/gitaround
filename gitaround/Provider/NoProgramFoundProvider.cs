using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gitaround.Provider
{
    internal class NoProgramFoundProvider : IProgram
    {
        private readonly ILogger _logger;

        public NoProgramFoundProvider(ILogger logger)
        {
            _logger = logger;
        }

        public void Run()
        {
            _logger.Error(nameof(NoProgramFoundProvider), $"Missing command arguments. Exit.");
        }
    }
}
