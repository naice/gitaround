using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace gitaround.Parseable
{
    internal class SourceTree : Parseable
    {
        internal enum ProcessType { None, CheckOutRef }
        public ProcessType Process { get; set; } = ProcessType.None;
        public override string Ref { get; set; }
        public override string CloneUrl { get; set; }
        public string Type { get; set; }
        public string BaseWebUrl { get; set; }
        public string User { get; set; }

        private readonly Provider.ILogger _logger;
        public SourceTree(Provider.ILogger logger)
        {
            _logger = logger;
        }

        //sourcetree://checkoutRef?type=stash&ref=refs%2Fheads%2Fbugfix%2FPWSAL-942-alternative-buchungstermine-werden&baseWebUrl=https%3A%2F%2Fbitbucket.protel.net&cloneUrl=ssh%3A%2F%2Fgit%40bitbucket.protel.net%3A7999%2Fpcc%2Fpws.git&user=jmarchewka
        public override bool Parse(string input)
        {
            var uri = MakeUri(input);
            if (uri == null)
            {
                return false;
            }
            if (string.IsNullOrEmpty(uri.Scheme) || !uri.Scheme.Equals("sourcetree", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }
            if (string.IsNullOrEmpty(uri.Query))
            {
                _logger.Info(nameof(SourceTree), $"sourcetree scheme but no query. {uri.ToString()}");
                return false;
            }

            //_logger.Info(nameof(SourceTree), $"sourcetree scheme found: {uri.ToString()}");

            // checkoutRef
            if (!string.IsNullOrEmpty(uri.Host) && uri.Host.Equals("checkoutref", StringComparison.InvariantCultureIgnoreCase))
            {
                Process = ProcessType.CheckOutRef;
            }

            var query = HttpUtility.ParseQueryString(uri.Query);
            Type = query["type"];
            Ref = query["ref"];
            BaseWebUrl = query["baseWebUrl"];
            CloneUrl = query["cloneUrl"];
            User = query["user"];

            return true;
        }
    }
}
