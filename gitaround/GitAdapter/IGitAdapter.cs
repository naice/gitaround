using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gitaround.GitAdapter
{
    public class GitResult
    {
        public string Result { get; set; }
        public string Error { get; set; }
    }

    interface IGitAdapter
    {
        GitResult CheckoutBranch(string projectPath, string branchName, string remoteName);
    }
}
