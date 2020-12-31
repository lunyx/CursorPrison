using CursorPrison.Messaging;
using CursorPrison.Messaging.Messages;
using CursorPrisonUtils;
using CursorPrisonUtils.Config;
using CursorPrisonWpf.Models;
using GalaSoft.MvvmLight.CommandWpf;
using Lunyx.Common.UI.Wpf;
using Lunyx.Common.UI.Wpf.Collections;
using Serilog;
using Omu.ValueInjecter;
using System.Collections.Generic;

namespace CursorPrisonWpf
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ForegroundWindowChangeProcessor processor;

        public MainWindowViewModel()
        {
            LoadConfig();

            CursorPrisonMessenger.Instance.Messenger.Register<ActiveProcessChangedMessage>(this, ActiveProcessChanged);
                        
            processor = new ForegroundWindowChangeProcessor();            
        }

        #region Properties

        public string ActiveProcessName
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        public SynchronizedObservableCollection<ProcessConfigModel> ProcessConfigs
        {
            get { return GetProperty(getDefault: () => new SynchronizedObservableCollection<ProcessConfigModel>()); }
            set { SetProperty(value); }
        }

        #endregion

        #region Commands

        public RelayCommand AddCurrentProcessCommand
        {
            get { return GetProperty(getDefault: () => new RelayCommand(AddCurrentProcess)); }
            set { SetProperty(value); }
        }

        public RelayCommand LoadConfigCommand
        {
            get { return GetProperty(getDefault: () => new RelayCommand(LoadConfig)); }
            set { SetProperty(value); }
        }

        public RelayCommand SaveConfigCommand
        {
            get { return GetProperty(getDefault: () => new RelayCommand(SaveConfig)); }
            set { SetProperty(value); }
        }

        #endregion

        #region Message Events

        private void ActiveProcessChanged(ActiveProcessChangedMessage message)
        {
            if (message.NewProcessName != nameof(CursorPrisonWpf))
                ActiveProcessName = message.NewProcessName;
        }

        #endregion
        
        private void LoadConfig()
        {
            ConfigManager.Instance.LoadConfig();
            ProcessConfigs.Clear();

            foreach (var pc in ConfigManager.Instance.Config.ProcessConfigs)
            {
                var model = new ProcessConfigModel();
                model.InjectFrom(pc);
                
                ProcessConfigs.Add(model);
            }
        }

        private void SaveConfig()
        {
            var processConfigs = new List<ProcessConfig>();
            foreach (var model in ProcessConfigs)
            {
                var pc = new ProcessConfig();
                pc.InjectFrom(model);

                processConfigs.Add(pc);
            }

            if (ConfigManager.Instance.SaveConfig(processConfigs))
            {
                LoadConfig();
            }
        }

        private void AddCurrentProcess()
        {
            var newProcess = ActiveProcessName;

            Log.Debug($"{nameof(AddCurrentProcess)}: {newProcess}");

            foreach (var pc in ProcessConfigs)
            {
                if (pc.ProcessName == newProcess)
                    return;
            }

            ProcessConfigs.Add(new ProcessConfigModel
            {
                ProcessName = newProcess
            });
        }
    }
}
