using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gitaround.Parseable
{
    internal interface IParseable
    {
        string Ref { get; set; }
        string CloneUrl { get; set; }
        bool Parse(string input);
    }
}
