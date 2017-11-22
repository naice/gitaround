using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gitaround.Model
{
    internal class CommandLineArgs
    {
        const string SEPERATOR = "=";
        const string PARAM_URL = "url";
        const string PARAM_UPDATE_REGISTRY = "update_registry";

        public string[] Args { get { return _args; } }
        public string Url { get { return this[PARAM_URL]; } }
        public bool IsUpdateRegistry { get { return _nameValueCollection.AllKeys.Contains(PARAM_UPDATE_REGISTRY); } }


        private readonly string[] _args;
        private readonly NameValueCollection _nameValueCollection;
        public CommandLineArgs(string[] args)
        {
            _args = args ?? throw new ArgumentException(nameof(args));
            _nameValueCollection = ParseNameValueCollection(args);
        }

        private NameValueCollection ParseNameValueCollection(string[] args)
        {
            var nvc = new NameValueCollection();
            
            foreach (var item in args)
            {
                string name, value;
                if (string.IsNullOrEmpty(item)) continue;

                var indexOfEq = item.IndexOf(SEPERATOR);
                if (indexOfEq <= 0)
                {
                    name = item;
                    value = null;
                }
                else
                {
                    name = item.Substring(0, indexOfEq).ToLowerInvariant();
                    value = item.Substring(indexOfEq + SEPERATOR.Length);
                }

                if (string.IsNullOrEmpty(value)) value = null;
                nvc.Add(name, value);
            }

            return nvc;
        }

        public string this[string name]
        {
            get
            {
                return _nameValueCollection[name];
            }
        }
        public string this[int index]
        {
            get
            {
                return _nameValueCollection[index];
            }
        }
    }
}
