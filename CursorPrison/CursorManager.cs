using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace CursorPrison
{
    public class CursorManager
    {
        private CursorManager()
        {
            
        }

        private static readonly Lazy<CursorManager> _instance = new Lazy<CursorManager>(() => new CursorManager());
        public static CursorManager Instance => _instance.Value;

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
            if (windowName == "Guild Wars 2")
            {
                RECT rect;
                GetWindowRect(hwnd, out rect);
                ClipCursor(ref rect);
            }
        }

        #region test
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        #endregion

        #region windowrect/cursor

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left, Top, Right, Bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            public RECT(System.Drawing.Rectangle r) : this(r.Left, r.Top, r.Right, r.Bottom)
            {
            }

            public int X
            {
                get { return Left; }
                set
                {
                    Right -= (Left - value);
                    Left = value;
                }
            }

            public int Y
            {
                get { return Top; }
                set
                {
                    Bottom -= (Top - value);
                    Top = value;
                }
            }

            public int Height
            {
                get { return Bottom - Top; }
                set { Bottom = value + Top; }
            }

            public int Width
            {
                get { return Right - Left; }
                set { Right = value + Left; }
            }

            public System.Drawing.Point Location
            {
                get { return new System.Drawing.Point(Left, Top); }
                set
                {
                    X = value.X;
                    Y = value.Y;
                }
            }

            public System.Drawing.Size Size
            {
                get { return new System.Drawing.Size(Width, Height); }
                set
                {
                    Width = value.Width;
                    Height = value.Height;
                }
            }

            public static implicit operator System.Drawing.Rectangle(RECT r)
            {
                return new System.Drawing.Rectangle(r.Left, r.Top, r.Width, r.Height);
            }

            public static implicit operator RECT(System.Drawing.Rectangle r)
            {
                return new RECT(r);
            }

            public static bool operator ==(RECT r1, RECT r2)
            {
                return r1.Equals(r2);
            }

            public static bool operator !=(RECT r1, RECT r2)
            {
                return !r1.Equals(r2);
            }

            public bool Equals(RECT r)
            {
                return r.Left == Left && r.Top == Top && r.Right == Right && r.Bottom == Bottom;
            }

            public override bool Equals(object obj)
            {
                if (obj is RECT)
                    return Equals((RECT) obj);
                else if (obj is System.Drawing.Rectangle)
                    return Equals(new RECT((System.Drawing.Rectangle) obj));
                return false;
            }

            public override int GetHashCode()
            {
                return ((System.Drawing.Rectangle) this).GetHashCode();
            }

            public override string ToString()
            {
                return string.Format(System.Globalization.CultureInfo.CurrentCulture,
                    "{{Left={0},Top={1},Right={2},Bottom={3}}}", Left, Top, Right, Bottom);
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool ClipCursor(ref RECT lpRect);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool ClipCursor(IntPtr zero);
        #endregion

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
