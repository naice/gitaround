using gitaround.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleInjector;

namespace gitaround.Factories
{

    /// <summary>
    /// Factory for <see cref="Configuration"/>.
    /// </summary>
    internal class ConfigurationFactory
    {
        internal static Configuration Factory(Container container)
        {
            Configuration config = null;
            ApplicationPath appPath = container.GetInstance<ApplicationPath>();
            var configPath = appPath.AppPathAndNameWithoutExtension + "-config.json";

            if (File.Exists(configPath))
            {
                try
                {
                    config = Newtonsoft.Json.JsonConvert.DeserializeObject<Configuration>(
                        File.ReadAllText(configPath));
                }
                catch (Newtonsoft.Json.JsonException ex)
                {
                    Trace.WriteLine($"Configuration file has a bad format. File: {configPath} Message: {ex.Message}");
                }
                catch (IOException ex)
                {
                    Trace.WriteLine($"Could not load configuration. File: {configPath} Message: {ex.Message}");
                }
            }
            else
            {
                config = new Configuration(appPath);
                try
                {
                    // try to create a config file for the user.
                    File.WriteAllText(configPath,
                        Newtonsoft.Json.JsonConvert.SerializeObject(config, Newtonsoft.Json.Formatting.Indented));
                }
                catch (IOException) { /* JUST EAT EXCEPTION */ }
                catch (Newtonsoft.Json.JsonException) { /* JUST EAT EXCEPTION */ }
            }

            return config ?? new Configuration(appPath);
        }
    }
}
