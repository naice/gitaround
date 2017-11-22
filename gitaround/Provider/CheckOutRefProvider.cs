using gitaround.Parseable;
using LibGit2Sharp;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gitaround.Provider
{
    internal class CheckOutRefProvider : IProgram
    {
        private readonly ILogger _logger;
        private readonly IEnumerable<IParseable> _parseable;
        private readonly Model.CommandLineArgs _commandLineArgs;
        private readonly Model.Configuration _configuration;

        public CheckOutRefProvider(IEnumerable<IParseable> parseable, Model.CommandLineArgs commandLineArgs, Model.Configuration configuration, ILogger logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _commandLineArgs = commandLineArgs ?? throw new ArgumentNullException(nameof(commandLineArgs));
            _parseable = parseable ?? throw new ArgumentNullException(nameof(parseable));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Run()
        {
            var url = _commandLineArgs.Url;
            //_logger.Info(nameof(CheckOutRefProvider), $"URL={url}");

            if (string.IsNullOrEmpty(url))
            {
                _logger.Error(nameof(CheckOutRefProvider), $"No Url given.");
                return;
            }

            var parsedInfo = _parseable.FirstOrDefault((parseable) => parseable.Parse(url));
            if (parsedInfo == null)
            {
                _logger.Error(nameof(CheckOutRefProvider), $"No parser matched Url: {url}");
                return;
            }

            if (string.IsNullOrEmpty(parsedInfo.Ref))
            {
                _logger.Error(nameof(CheckOutRefProvider), $"Parser matched but no Ref found. Url: {url}");
                return;
            }

            if (string.IsNullOrEmpty(parsedInfo.CloneUrl))
            {
                _logger.Error(nameof(CheckOutRefProvider), $"Parser matched but no CloneUrl found. Url: {url}");
                return;
            }

            var repositoryConfig = _configuration.Repositories.FirstOrDefault(crepo => crepo.CloneUrl == parsedInfo.CloneUrl);
            repositoryConfig.Remote = repositoryConfig.Remote ?? "origin";
            if (repositoryConfig == null)
            {
                _logger.Error(nameof(CheckOutRefProvider), $"No repository configuration found for remote: {parsedInfo.CloneUrl}");
                return;
            }

            var repositorySshCredential = _configuration.SshCredentials.FirstOrDefault(ccred => ccred.User == repositoryConfig.User);
            if (repositorySshCredential == null)
            {
                _logger.Error(nameof(CheckOutRefProvider), $"No ssh credentials found for User {repositoryConfig.User}. No Fetch.");
            }

            // create branch names
            var branchName = parsedInfo.Ref.Replace("refs/heads/", "");
            var remoteName = repositoryConfig.Remote;
            var repositoryPath = repositoryConfig.Local;
            
            using (var repo = new Repository(repositoryPath))
            {
                if (repositorySshCredential != null)
                {
                    _logger.Info(nameof(CheckOutRefProvider), $"FetchAll from {repositoryConfig.CloneUrl}.");
                    FetchAll(repo,
                        repositorySshCredential.Passphrase,
                        repositorySshCredential.PrivateKeyFile,
                        repositorySshCredential.PublicKeyFile,
                        "git");
                }

                _logger.Info(nameof(CheckOutRefProvider), $"Checkout branch {branchName}.");
                CheckoutBranch(repo, branchName, remoteName);
            }
            _logger.Info(nameof(CheckOutRefProvider), $"Done.");
        }

        private Branch CheckoutBranch(Repository repo, string branchName, string remoteName)
        {
            var localBranchCanonicalName = $"refs/heads/{branchName}";
            var remoteBranchCanonicalName = $"refs/remotes/{remoteName}/{branchName}";


            var remoteBranch = repo.Branches.FirstOrDefault((rbranch) => rbranch.CanonicalName == remoteBranchCanonicalName);
            var localBranch = repo.Branches.FirstOrDefault((rbranch) => rbranch.CanonicalName == localBranchCanonicalName);

            if (remoteBranch == null)
            {
                _logger.Error(nameof(CheckOutRefProvider), $"Remote branch not found. {remoteBranchCanonicalName}");
                return null;
            }

            if (localBranch == null)
            {
                localBranch = repo.Branches.Add(branchName, remoteBranch.Tip);
                repo.Branches.Update(localBranch, p => p.TrackedBranch = remoteBranch.CanonicalName);
            }

            return Commands.Checkout(repo, localBranch, new CheckoutOptions() { CheckoutModifiers = CheckoutModifiers.None });
        }

        private void FetchAll(Repository repo, string passphrase, string privateKey, string publicKey, string userName)
        {
            var sshCred = new SshUserKeyCredentials()
            {
                Passphrase = passphrase,
                PrivateKey = privateKey,
                PublicKey = publicKey,
                Username = userName,
            };

            FetchOptions options = new FetchOptions()
            {
                CredentialsProvider = new LibGit2Sharp.Handlers.CredentialsHandler((_, __, ___) => sshCred),
            };

            foreach (Remote remote in repo.Network.Remotes)
            {
                IEnumerable<string> refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);

                try
                {
                    Commands.Fetch(repo, remote.Name, refSpecs, options, "");
                }
                catch (Exception ex)
                {
                    _logger.Error(nameof(CheckOutRefProvider), $"Could not fetch all on \"{remote.Url}\". {ex.Message}");
                }
            }
        }
    }
}
