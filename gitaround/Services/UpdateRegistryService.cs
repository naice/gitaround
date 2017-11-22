using System;
using Microsoft.Win32;
using System.IO;
using SimpleInjector;
using gitaround.Provider;

namespace gitaround.Services
{
    internal class UpdateRegistryProvider : IService
    {
        const string MY_KEY = "sourcetree";
        private readonly ILogger _logger;
        private readonly Model.ApplicationPath _appPath;
        public UpdateRegistryProvider(ILogger logger, Model.ApplicationPath appPath)
        {
            _logger = logger;
            _appPath = appPath;
        }


        public void Run()
        {
            var classesRoot = Registry.ClassesRoot;
            using (var sourceTreeKey = classesRoot.OpenSubKey(MY_KEY))
            {
                if (sourceTreeKey != null)
                {
                    _logger.Info(nameof(UpdateRegistryProvider), $"sourcetree key found, delete.");

                    try
                    {
                        classesRoot.DeleteSubKeyTree(MY_KEY);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        _logger.Info(nameof(UpdateRegistryProvider), $"Can not delete sourcetree key, need higher privileges. {ex.Message}");
                    }
                }

                try
                {
                    _logger.Info(nameof(UpdateRegistryProvider), $"creating new sourcetree key");
                    RegisterSourceTreeHookCustomURIScheme(classesRoot);
                }
                catch (UnauthorizedAccessException ex)
                {
                    _logger.Error(nameof(UpdateRegistryProvider), $"Can not create SourceTree Custom scheme hook. {ex.Message}");
                }
            }

            _logger.Info(nameof(UpdateRegistryProvider), $"Done.");
            Console.Read();
        }

        private void RegisterSourceTreeHookCustomURIScheme(RegistryKey classesRoot)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var app = assembly.Location;
            var command = $"\"{app}\" url=\"%1\"";
            var defaultIcon = $"{app},1";

            using (var myKey = classesRoot.CreateSubKey(MY_KEY, true))
            {
                myKey.SetValue(null, "URL:sourcetree Protocol");
                myKey.SetValue("URL Protocol", "");
                using (var defaultIconKey = myKey.CreateSubKey("DefaultIcon", true))
                {
                    defaultIconKey.SetValue(null, defaultIcon);
                }
                using (var shellKey = myKey.CreateSubKey("shell", true))
                {
                    using (var openKey = shellKey.CreateSubKey("open", true))
                    {
                        using (var commandKey = openKey.CreateSubKey("command", true))
                        {
                            commandKey.SetValue(null, command);
                        }
                    }
                }
            }
        }
    }
}