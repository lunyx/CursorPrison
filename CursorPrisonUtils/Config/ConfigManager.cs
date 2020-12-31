using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;

namespace CursorPrisonUtils.Config
{
    public class ConfigManager
    {
        private static readonly Lazy<ConfigManager> Lazy = new Lazy<ConfigManager>(() => new ConfigManager());
        private static readonly string ConfigPath = Path.Combine(Environment.GetEnvironmentVariable("AppData"), "CursorPrison", "config.json");

        public CursorPrisonConfig Config { get; private set; } = new CursorPrisonConfig();

        public static ConfigManager Instance => Lazy.Value;

        private ConfigManager() 
        { 
        }

        public bool LoadConfig()
        {
            if (!File.Exists(ConfigPath))
            {
                Directory.CreateDirectory(Path.Combine(Environment.GetEnvironmentVariable("AppData"), "CursorPrison"));
                File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(new CursorPrisonConfig()));
            }

            try
            {
                Config = JsonConvert.DeserializeObject<CursorPrisonConfig>(File.ReadAllText(ConfigPath));
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to load config.");
                return false;
            }
        }

        public bool SaveConfig(List<ProcessConfig> processConfigs)
        {
            var config = new CursorPrisonConfig
            {
                ProcessConfigs = processConfigs
            };

            try
            {
                File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(config));
                return true;
            } catch (Exception ex)
            {
                Log.Error(ex, "Failed to save config.");
                return false;
            }
            
        }
    }
}
