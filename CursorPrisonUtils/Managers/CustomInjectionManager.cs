using CursorPrison.Extensibility;
using CursorPrisonUtils.Config;
using CursorPrisonUtils.Contracts;
using Lamar;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CursorPrisonUtils.Managers
{
    public class CustomInjectionManager : IChangeManager
    {
        private Dictionary<string, Container> _injections = new Dictionary<string, Container>();
        private Dictionary<Type, InjectableKeyboardHook> _kbHooks = new Dictionary<Type, InjectableKeyboardHook>();
        private readonly Func<InjectableKeyboardHook> _kbHookFactory;

        private static readonly object InjectionBuildLock = new object();

        public CustomInjectionManager(Func<InjectableKeyboardHook> kbHookFactory)
        {
            _kbHookFactory = kbHookFactory;
        }

        public void HandleForegroundWindowChange(string processName, IntPtr hwnd)
        {
            var dict = ConfigManager.Instance.Config.ProcessConfigs.Where(c => c.CustomInjectionPath != null && Directory.Exists(c.CustomInjectionPath))
                .ToDictionary(c => c.ProcessName, c => c.CustomInjectionPath);

            foreach (var processInjection in dict)
            {
                if (!_injections.ContainsKey(processInjection.Value))
                {   // load injections if not loaded already
                    lock (InjectionBuildLock)
                    {
                        var container = new Container(c =>
                        {
                            c.Scan(x =>
                            {
                                x.AssembliesFromPath(processInjection.Value);
                                x.AddAllTypesOf<ICustomContextChangeAction>(ServiceLifetime.Singleton);
                            });
                        });
                        {
                            _injections[processInjection.Value] = container;
                            foreach (var injection in _injections[processInjection.Value].GetAllInstances<ICustomContextChangeAction>())
                            {
                                if (injection is ICustomContextChangeActionWithKeyboardHook && !_kbHooks.ContainsKey(injection.GetType()))
                                {
                                    var kbHookInjection = injection as ICustomContextChangeActionWithKeyboardHook;
                                    var kbHook = _kbHookFactory();

                                    kbHookInjection.InitializeKeyboardHook(kbHook);
                                    _kbHooks[injection.GetType()] = kbHook;
                                }
                            }
                        }
                    }
                }

                foreach (var injection in _injections[processInjection.Value].GetAllInstances<ICustomContextChangeAction>())
                {
                    try
                    {
                        if (processName == processInjection.Key)
                            injection.Activate();
                        else
                            injection.Deactivate();
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Error in custom injection: activeProcess:{processName} targetProcess:{processInjection.Key} class:{injection.GetType().Name}. {ex}");
                    }
                }
            }
        }
    }
}
