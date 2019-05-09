using gitaround.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gitaround.GitAdapter
{
    public class GitConsoleAdapter : IGitAdapter
    {
        private readonly Configuration _config;

        public GitConsoleAdapter(Configuration configuration)
        {
            _config = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public GitResult CheckoutBranch(string projectPath, string branchName, string remoteName)
        {
            var reader = new GitReader(_config.GitExecutable, projectPath, $"checkout -t {remoteName}/{branchName}");
            
            reader.Run();

            if (reader.Error?.Trim().EndsWith($"A branch named '{branchName}' already exists.") ?? false)
            {
                reader = new GitReader(_config.GitExecutable, projectPath, $"checkout {branchName}");
                reader.Run();
            }

            return new GitResult() { Error = reader.Error, Result = reader.Result };
        }
    }


    internal class GitReader
    {
        private readonly string _gitPath;
        private readonly string _repoPath;
        private readonly string _commands;

        public string Error { get; private set; }
        public string Result { get; private set; }

        public GitReader(string gitPath, string repoPath, string command)
        {
            _gitPath = gitPath;
            _repoPath = repoPath;
            _commands = command;
        }

        public void Run()
        {
            Error = null;
            Result = null;
            using (Process p = new Process())
            {
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.WorkingDirectory = _repoPath;
                p.StartInfo.FileName = _gitPath;
                p.StartInfo.Arguments = _commands;
                p.Start();
                string stderr_str = "";//p.StandardError.ReadToEnd();
                string stdout_str = "";//p.StandardOutput.ReadToEnd();

                p.OutputDataReceived += (sender, e) =>
                {
                    if (!String.IsNullOrEmpty(e.Data))
                    {
                        stdout_str += e.Data + Environment.NewLine;
                    }
                };
                p.ErrorDataReceived += (sender, e) =>
                {
                    if (!String.IsNullOrEmpty(e.Data))
                    {
                        stderr_str += e.Data + Environment.NewLine;
                    }
                };

                p.Start();

                p.BeginErrorReadLine();
                p.BeginOutputReadLine();

                p.WaitForExit();
                p.Close();
                Error = string.IsNullOrEmpty(stderr_str) ? null : stderr_str;
                Result = stdout_str;
            }
        }
    }
}
