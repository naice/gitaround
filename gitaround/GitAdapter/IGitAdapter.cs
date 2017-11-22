using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gitaround.GitAdapter
{
    interface IGitAdapter
    {
        void OpenRepository(string path);
        void CloseRepository();
        void FetchAll(string passphrase, string privateKey, string publicKey, string userName);
        void CheckoutBranch(string branchName, string remoteName);
    }
}
