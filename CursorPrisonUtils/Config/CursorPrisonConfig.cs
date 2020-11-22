using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursorPrisonUtils.Config
{
    public class CursorPrisonConfig
    {
        public List<ProcessConfig> ProcessConfigs { get; set; }
        public int BorderlessXOffset { get; set; }
        public int BorderlessYOffset { get; set; }
    }
}
