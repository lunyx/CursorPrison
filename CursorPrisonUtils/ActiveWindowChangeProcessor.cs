using CursorPrisonUtils.Contracts;
using CursorPrisonUtils.Managers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CursorPrisonUtils
{
    public class ActiveWindowChangeProcessor
    {
        private readonly WinEventDelegate _dele;
        private readonly List<IChangeManager> _changeManagers;

        public ActiveWindowChangeProcessor()
        {
            _changeManagers = new List<IChangeManager>
            {
                new WindowManager(),
                new SoundManager()
            };

            _dele = new WinEventDelegate(WinEventProc);
            IntPtr m_hhook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, _dele, 0, 0, WINEVENT_OUTOFCONTEXT);
        }

        public void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            GetWindowThreadProcessId(hwnd, out var pid);
            var processName = Process.GetProcessById((int)pid).ProcessName;
            Debug.WriteLine(processName);

            foreach (var manager in _changeManagers)
            {
                Task.Run(() => manager.HandleForegroundWindowChange(processName, hwnd));
            }
        }

        delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint EVENT_SYSTEM_FOREGROUND = 3;

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
                
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowModuleFileName(IntPtr hwnd, StringBuilder lpszFileName, uint cchFileNameMax);
    }
}
