using Lunyx.Common.UI.Wpf;

namespace CursorPrisonWpf.Models
{
    public class ProcessConfigModel : ViewModelBase
    {
        public string ProcessName
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        public bool BindCursorArea
        {
            get { return GetProperty<bool>(); }
            set { SetProperty(value); }
        }

        public bool BorderlessWindow
        {
            get { return GetProperty<bool>(); }
            set { SetProperty(value); }
        }

        public bool BackgroundMute
        {
            get { return GetProperty<bool>(); }
            set { SetProperty(value); }
        }
    }
}
