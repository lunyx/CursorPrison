using CursorPrisonUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursorPrisonWpf
{
    public class MainWindowViewModel
    {
        private ActiveWindowChangeProcessor processor;

        public MainWindowViewModel()
        {
            processor = new ActiveWindowChangeProcessor();
        }

    }
}
