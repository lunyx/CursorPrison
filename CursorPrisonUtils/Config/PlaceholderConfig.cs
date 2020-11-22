using System.Collections.Generic;

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
                },
                new ProcessConfig
                {
                    ProcessName = "Gw2-64",
                    BindCursorArea = true
                }
            }
        };
    }
}
