using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CursorPrison
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new CursorPrisonApplicationContext());
        }
    }


    public class CursorPrisonApplicationContext : ApplicationContext
    {
        private readonly NotifyIcon _trayIcon;

        public CursorPrisonApplicationContext()
        {
            // Initialize Tray Icon
            _trayIcon = new NotifyIcon
            {
                ContextMenu = new ContextMenu(new[]
                {
                    new MenuItem("Exit", Exit)
                }),
                Visible = true
            };

            Task.Factory.StartNew(() => CursorManager.Instance.Initialize(), TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(() => WindowManager.Instance.Initialize(), TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(() => SoundManager.Instance.Initialize(), TaskCreationOptions.LongRunning);
        }

        void Exit(object sender, EventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            _trayIcon.Visible = false;

            Application.Exit();
        }
    }
}
