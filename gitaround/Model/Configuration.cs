﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace gitaround.Model
{
    public class Configuration
    {
        public string GitExecutable { get; set; } = @"C:\Program Files\Git\bin\git.exe";
        public string LogFilePath { get; set; }
        public List<Repository> Repositories { get; set; } = new List<Repository>() { new Repository() };
        public List<SshCredentials> SshCredentials { get; set; } = new List<SshCredentials>() { new SshCredentials() };

        public Configuration()
        {

        }
        public Configuration(ApplicationPath path)
        {
            LogFilePath = "%APPPATH%\\" + path.AppNameWithoutExtension + ".log";
        }
    }
}
