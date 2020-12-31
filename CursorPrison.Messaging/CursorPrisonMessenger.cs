using CursorPrison.Messaging.Messages;
using GalaSoft.MvvmLight.Messaging;
using System;

namespace CursorPrison.Messaging
{
    public sealed class CursorPrisonMessenger
    {
        private static readonly Lazy<CursorPrisonMessenger> Lazy = new Lazy<CursorPrisonMessenger>(() => new CursorPrisonMessenger());

        public static CursorPrisonMessenger Instance => Lazy.Value;

        public IMessenger Messenger { get { return GalaSoft.MvvmLight.Messaging.Messenger.Default; } }

        private CursorPrisonMessenger()
        {
        }

        public void ActiveProcessChanged(string newProcessName)
        {
            Messenger.Send(new ActiveProcessChangedMessage
            {
                NewProcessName = newProcessName
            });
        }
    }
}
