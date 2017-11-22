using gitaround.Provider;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gitaround.GitAdapter
{
    internal class LibGit2SharpGitAdapter : IGitAdapter
    {
        private readonly ILogger _logger;
        private Repository _repository;

        public LibGit2SharpGitAdapter(ILogger logger)
        {
            _logger = logger;
        }

        public void CheckoutBranch(string branchName, string remoteName)
        {
            CheckoutBranch(_repository, branchName, remoteName);
        }

        public void CloseRepository()
        {
            if (_repository != null)
            {
                _repository.Dispose();
                _repository = null;
            }
        }

        public void FetchAll(string passphrase, string privateKey, string publicKey, string userName)
        {
            FetchAll(_repository, passphrase, privateKey, publicKey, userName);
        }

        public void OpenRepository(string path)
        {
            if (_repository != null) throw new InvalidOperationException("Branch already open!");
            _repository = new Repository(path);
        }
        
        private Branch CheckoutBranch(Repository repo, string branchName, string remoteName)
        {
            var localBranchCanonicalName = $"refs/heads/{branchName}";
            var remoteBranchCanonicalName = $"refs/remotes/{remoteName}/{branchName}";


            var remoteBranch = repo.Branches.FirstOrDefault((rbranch) => rbranch.CanonicalName == remoteBranchCanonicalName);
            var localBranch = repo.Branches.FirstOrDefault((rbranch) => rbranch.CanonicalName == localBranchCanonicalName);

            if (remoteBranch == null)
            {
                _logger.Error(nameof(LibGit2SharpGitAdapter), $"Remote branch not found. {remoteBranchCanonicalName}");
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
                    _logger.Error(nameof(LibGit2SharpGitAdapter), $"Could not fetch all on \"{remote.Url}\". {ex.Message}");
                }
            }
        }
    }
}
