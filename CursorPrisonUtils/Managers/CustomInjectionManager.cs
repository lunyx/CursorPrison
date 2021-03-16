using CursorPrison.Extensibility;
using CursorPrisonUtils.Config;
using CursorPrisonUtils.Contracts;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CursorPrisonUtils.Managers
{
    public class CustomInjectionManager : IChangeManager
    {
        private Dictionary<string, List<ICustomContextChangeAction>> _injections = new Dictionary<string, List<ICustomContextChangeAction>>();

        public void HandleForegroundWindowChange(string processName, IntPtr hwnd)
        {
            var dict = ConfigManager.Instance.Config.ProcessConfigs.Where(c => c.CustomInjectionPath != null && Directory.Exists(c.CustomInjectionPath))
                .ToDictionary(c => c.ProcessName, c => c.CustomInjectionPath);

            foreach (var path in dict.Values.Where(p => !_injections.ContainsKey(p))) 
            {   // load injections if not already loaded

            }

            foreach (var processInjection in dict)
            {
                if (!_injections.ContainsKey(processInjection.Value))
                {   // load injections if not loaded already

                }

                foreach (var injection in _injections[processInjection.Value])
                {
                    try
                    {
                        if (processName == processInjection.Key)
                            injection.Activate();
                        else
                            injection.Deactivate();
                    } catch (Exception ex)
                    {
                        Log.Error($"Error in custom injection: activeProcess:{processName} targetProcess:{processInjection.Key} class:{injection.GetType().Name}. {ex}");
                    }
            }
        }
    }
}
