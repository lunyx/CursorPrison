using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CursorPrison
{
    public class WindowManager
    {
        private WindowManager()
        {

        }

        private static readonly Lazy<WindowManager> _instance = new Lazy<WindowManager>(() => new WindowManager());
        public static WindowManager Instance => _instance.Value;

        public void Initialize()
        {
            while (true)
            {
                ProcessEvent(GetForegroundWindow());
                Thread.Sleep(5);
            }
            //hook to window change event
            //dele = WinEventProc;
            //SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, dele, 0, 0, WINEVENT_OUTOFCONTEXT);
        }

        private void ProcessEvent(IntPtr hwnd)
        {
            var windowName = GetActiveWindowTitle(hwnd);
            if (windowName == "Genshin Impact")
            {
                // Move the window to (0,0) without changing its size or position
                // in the Z order.
                SetWindowPos(hwnd, IntPtr.Zero, -3, -26, 0, 0, SWP_NOSIZE | SWP_NOZORDER);
            }
        }

        #region test
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        #endregion


        // P/Invoke declarations.

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        const uint SWP_NOSIZE = 0x0001;
        const uint SWP_NOZORDER = 0x0004;

        #region windowchange
        WinEventDelegate dele = null;

        delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint EVENT_SYSTEM_FOREGROUND = 3;


        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        private string GetActiveWindowTitle(IntPtr hwnd)
        {
            const int nChars = 256;
            StringBuilder buff = new StringBuilder(nChars);

            if (GetWindowText(hwnd, buff, nChars) > 0)
            {
                return buff.ToString();
            }
            return null;
        }

        public void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            ProcessEvent(hwnd);
        }
        #endregion
    }
}
