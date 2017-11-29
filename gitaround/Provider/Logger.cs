using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gitaround.Provider
{
    internal interface ILogger
    {
        void Info(string where, string msg);
        void Error(string where, string msg);

        void WriteLine(string msg);
        void Write(string msg);
    }
    internal class Logger : ILogger
    {
        private readonly Model.Configuration _configuration;
        private readonly Model.ApplicationPath _appPath;
        private readonly Lazy<string> _logFilePath;

        public Logger(Model.Configuration configuration, Model.ApplicationPath appPath)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _appPath = appPath ?? throw new ArgumentNullException(nameof(appPath));
            _logFilePath = new Lazy<string>(() => _appPath.Expand(_configuration.LogFilePath));
        }

        public void Error(string where, string msg)
        {
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            WriteLineFormatted("Error", where, msg);
            Console.ForegroundColor = currentColor;
        }

        public void Info(string where, string msg)
        {
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Gray;
            WriteLineFormatted("Info", where, msg);
            Console.ForegroundColor = currentColor;
        }

        private void WriteLineFormatted(string type, string where, string msg)
        {
            WriteLine($"[{DateTime.Now:yyyy-MM-ddTHH:mm:ss.fffffff}] [{type,10}] {where}: {msg}");
        }

        public void Write(string msg)
        {
            Trace.Write(msg);
            Console.Write(msg);
            AppendToLogFile(msg);
        }

        public void WriteLine(string msg)
        {
            Write(msg + Environment.NewLine);
        }
        private void AppendToLogFile(string msg)
        {
            var logFilePath = _logFilePath.Value;

            if (string.IsNullOrEmpty(logFilePath))
            {
                return;
            }

            try
            {
                File.AppendAllText(logFilePath, msg);
            }
            catch (IOException ex)
            {
                Error(nameof(Logger), $"Could not write to log file \"{logFilePath}\". {ex.Message}");
                Environment.Exit(100);
            }
        }
    }
}
