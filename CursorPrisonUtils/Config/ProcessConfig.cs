using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursorPrisonUtils.Config
{
    public class ProcessConfig
    {
        public string ProcessName { get; set; }
        public bool BindCursorArea { get; set; }
        public bool BorderlessWindow { get; set; }
        public bool BackgroundMute { get; set; }
    }
}
