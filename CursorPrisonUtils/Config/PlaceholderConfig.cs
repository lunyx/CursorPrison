using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursorPrisonUtils.Config
{
    public static class PlaceholderConfig
    {
        public static CursorPrisonConfig Value = new CursorPrisonConfig
        {
            BorderlessXOffset = -3,
            BorderlessYOffset = -26,
            ProcessConfigs = new List<ProcessConfig>
            {
                new ProcessConfig
                {
                    ProcessName = "GenshinImpact",
                    BorderlessWindow = true,
                    BackgroundMute = true
                }
            }
        };
    }
}
