using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Lunyx.Common.UI.Wpf;
using Serilog;

namespace CursorPrisonWpf
{               
    public partial class App : ISingleInstanceApp
    {
        private const string Unique = "eb46bc6a-815e-4daa-a0d2-79d9999e4886";

        [STAThread]
        public static void Main()
        {
            if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel
#if DEBUG
                    .Debug()
#else
                    .Information()
#endif
                    .WriteTo.File(
                        Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), "CursorPrison", "log.txt"),
                        rollOnFileSizeLimit: true,
                        fileSizeLimitBytes: 10000000,
                        retainedFileCountLimit: 5)
                    .CreateLogger();

                Log.Debug("Starting up.");

                Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mzc2MDU5QDMxMzgyZTM0MmUzMGdaaFlpdzlTaytqUmIyOXI1aUpNSXBTNmlLNEhkZEVVR28xcDZObTk1Qnc9");

                var application = new App();
                application.InitializeComponent();
                application.ShutdownMode = ShutdownMode.OnMainWindowClose;

                // register unhandled exceptions
                AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
                Current.DispatcherUnhandledException += CurrentOnDispatcherUnhandledException;
                TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

                application.Run();

                Log.Debug("Closing.");
                SingleInstance<App>.Cleanup();
                Environment.Exit(0);
            }
        }

        private static void HandleException(Exception e)
        {
            if (e == null) return;
            if (e.InnerException != null)
            {
                HandleException(e.InnerException);
            }
            else
            {
                Log.Error($"Unhandled exception: {e}");
            }
        }

        private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            HandleException(e.Exception);
        }

        private static void CurrentOnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            HandleException(e.Exception);
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException(e.ExceptionObject as Exception);
            if (e.IsTerminating)
                MessageBox.Show("There was an unexpected error. Please check the log for more details.", "CursorPrison",
                    MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            // handle command line arguments of second instance
            // ...
            return true;
        }
    }
}
