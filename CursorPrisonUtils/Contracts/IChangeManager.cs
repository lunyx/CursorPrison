using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursorPrisonUtils.Contracts
{
    public interface IChangeManager
    {
        void HandleForegroundWindowChange(string processName, IntPtr hwnd);
    }
}
