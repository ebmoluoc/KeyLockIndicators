using KeyLockIndicators.Services.Ipc;
using KeyLockIndicators.Services.KeyboardHook;
using KeyLockIndicators.Services.NotifyIcons;
using Microsoft.Win32;
using System;
using System.Windows.Forms;

namespace KeyLockIndicators.Notifier
{

    internal sealed class TaskbarNotification : ApplicationContext
    {

        private readonly INotifyIcons _notifyIcons;


        public TaskbarNotification(IIpcClient ipcClient, INotifyIcons notifyIcons, IKeyboardHook keyboardHook)
        {
            SystemEvents.SessionEnding += OnCloseEvent;
            ipcClient.CloseEvent += OnCloseEvent;

            _notifyIcons = notifyIcons;

            keyboardHook.KeyboardHookCapsLockEvent += OnKeyboardHookCapsLockEvent;
            _notifyIcons.ShowCapsLockIcon(Helpers.GetCapsLockState());

            keyboardHook.KeyboardHookNumLockEvent += OnKeyboardHookNumLockEvent;
            _notifyIcons.ShowNumLockIcon(Helpers.GetNumLockState());

            keyboardHook.KeyboardHookScrollLockEvent += OnKeyboardHookScrollLockEvent;
            _notifyIcons.ShowScrollLockIcon(Helpers.GetScrollLockState());
        }


        protected override void Dispose(bool disposing)
        {
            SystemEvents.SessionEnding -= OnCloseEvent;
            base.Dispose(disposing);
        }


        private void OnKeyboardHookCapsLockEvent(object sender, KeyboardHookEventArgs e)
        {
            _notifyIcons.ShowCapsLockIcon(e.LockState);
        }


        private void OnKeyboardHookNumLockEvent(object sender, KeyboardHookEventArgs e)
        {
            _notifyIcons.ShowNumLockIcon(e.LockState);
        }


        private void OnKeyboardHookScrollLockEvent(object sender, KeyboardHookEventArgs e)
        {
            _notifyIcons.ShowScrollLockIcon(e.LockState);
        }


        private void OnCloseEvent(object sender, EventArgs e)
        {
            ExitThread();
        }

    }

}
