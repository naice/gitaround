using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gitaround.Parseable
{
    internal abstract class Parseable : IParseable
    {
        public abstract string Ref { get; set; }
        public abstract string CloneUrl { get; set; }

        public abstract bool Parse(string input);

        protected static Uri MakeUri(string uri)
        {
            try
            {
                if (Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute))
                {
                    return new Uri(uri);
                }
            }
            catch (UriFormatException)
            {
            }

            return null;
        }
    }
}
