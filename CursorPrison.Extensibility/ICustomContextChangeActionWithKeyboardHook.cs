namespace CursorPrison.Extensibility
{
    public interface ICustomContextChangeActionWithKeyboardHook : ICustomContextChangeAction
    {
        void InitializeKeyboardHook(InjectableKeyboardHook kbHook);
    }
}
