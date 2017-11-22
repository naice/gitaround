using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gitaround.Model
{
    internal class SshCredentials
    {
        public string PrivateKeyFile { get; set; } = "";
        public string PublicKeyFile { get; set; } = "";
        public string User { get; set; } = "";
        public string Passphrase { get; set; } = "";
    }
}
