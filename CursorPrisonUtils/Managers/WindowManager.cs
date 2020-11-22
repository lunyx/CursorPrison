using CursorPrisonUtils.Config;
using CursorPrisonUtils.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CursorPrisonUtils.Managers
{
    public class WindowManager : IChangeManager
    {
        public void HandleForegroundWindowChange(string processName, IntPtr hwnd)
        {
            var dict = PlaceholderConfig.Value.ProcessConfigs.ToDictionary(c => c.ProcessName, c => c.BorderlessWindow);
            if (dict.ContainsKey(processName) && dict[processName])
            {
                // Move the window to (0,0) without changing its size or position
                // in the Z order.
                SetWindowPos(hwnd, IntPtr.Zero, -3, -26, 0, 0, SWP_NOSIZE | SWP_NOZORDER);
            }
        }
                
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        const uint SWP_NOSIZE = 0x0001;
        const uint SWP_NOZORDER = 0x0004;
    }
}
