using gitaround.Parseable;
using gitaround.Provider;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gitaround.Services
{
    internal class CheckOutRefService : IService
    {
        private readonly ILogger _logger;
        private readonly IEnumerable<IParseable> _parseable;
        private readonly Model.CommandLineArgs _commandLineArgs;
        private readonly Model.Configuration _configuration;
        private readonly GitAdapter.IGitAdapter _gitAdapter;

        public CheckOutRefService(IEnumerable<IParseable> parseable, Model.CommandLineArgs commandLineArgs, Model.Configuration configuration, ILogger logger, GitAdapter.IGitAdapter gitAdapter)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _commandLineArgs = commandLineArgs ?? throw new ArgumentNullException(nameof(commandLineArgs));
            _parseable = parseable ?? throw new ArgumentNullException(nameof(parseable));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _gitAdapter = gitAdapter ?? throw new ArgumentNullException(nameof(gitAdapter));
        }

        public void Run()
        {
            var url = _commandLineArgs.Url;
            //_logger.Info(nameof(CheckOutRefProvider), $"URL={url}");

            if (string.IsNullOrEmpty(url))
            {
                _logger.Error(nameof(CheckOutRefService), $"No Url given.");
                return;
            }

            var parsedInfo = _parseable.FirstOrDefault((parseable) => parseable.Parse(url));
            if (parsedInfo == null)
            {
                _logger.Error(nameof(CheckOutRefService), $"No parser matched Url: {url}");
                return;
            }

            if (string.IsNullOrEmpty(parsedInfo.Ref))
            {
                _logger.Error(nameof(CheckOutRefService), $"Parser matched but no Ref found. Url: {url}");
                return;
            }

            if (string.IsNullOrEmpty(parsedInfo.CloneUrl))
            {
                _logger.Error(nameof(CheckOutRefService), $"Parser matched but no CloneUrl found. Url: {url}");
                return;
            }

            var repositoryConfig = _configuration.Repositories.FirstOrDefault(crepo => crepo.CloneUrl == parsedInfo.CloneUrl);
            repositoryConfig.Remote = repositoryConfig.Remote ?? "origin";
            if (repositoryConfig == null)
            {
                _logger.Error(nameof(CheckOutRefService), $"No repository configuration found for remote: {parsedInfo.CloneUrl}");
                return;
            }

            var repositorySshCredential = _configuration.SshCredentials.FirstOrDefault(ccred => ccred.User == repositoryConfig.User);
            if (repositorySshCredential == null)
            {
                _logger.Error(nameof(CheckOutRefService), $"No ssh credentials found for User {repositoryConfig.User}. No Fetch.");
            }

            // create branch names
            var branchName = parsedInfo.Ref.Replace("refs/heads/", "");
            var remoteName = repositoryConfig.Remote;
            var repositoryPath = repositoryConfig.Local;


            try
            {
                _gitAdapter.OpenRepository(repositoryPath);
                if (repositorySshCredential != null)
                {
                    _logger.Info(nameof(CheckOutRefService), $"FetchAll from {repositoryConfig.CloneUrl}.");
                    _gitAdapter.FetchAll(
                        repositorySshCredential.Passphrase,
                        repositorySshCredential.PrivateKeyFile,
                        repositorySshCredential.PublicKeyFile,
                        "git");
                }

                _logger.Info(nameof(CheckOutRefService), $"Checkout branch {branchName}.");
                _gitAdapter.CheckoutBranch(branchName, remoteName);
            }
            catch (Exception ex)
            {
                _logger.Error(nameof(CheckOutRefService), $"Error while processing git commands. {ex.Message}");
            }
            finally
            {
                _gitAdapter.CloseRepository();
            }


            _logger.Info(nameof(CheckOutRefService), $"Done.");
        }

    }
}
