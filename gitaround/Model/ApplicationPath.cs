using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace gitaround.Model
{
    internal class ApplicationPath
    {
        public string App { get; set; }
        public string AppNameWithoutExtension { get; set; }
        public string AppName { get; set; }
        public string AppPath { get; set; }
        public string AppPathAndNameWithoutExtension => Path.Combine(AppPath, AppNameWithoutExtension);

        public ApplicationPath()
        {
            var assembly = Assembly.GetExecutingAssembly();
            App = assembly.Location;
            AppNameWithoutExtension = Path.GetFileNameWithoutExtension(App);
            AppName = Path.GetFileName(App);
            AppPath = Path.GetDirectoryName(App);
        }

        public string CombineAppDirectory(params string[] paths)
        {
            var npath = new string[paths.Length + 1];
            npath[0] = AppPath;
            paths.CopyTo(npath, 1);
            return Path.Combine(npath);
        }

        public string Expand(string path)
        {
            return path?
                .Replace("%APPPATH%", AppPath);
        }
    }
}
