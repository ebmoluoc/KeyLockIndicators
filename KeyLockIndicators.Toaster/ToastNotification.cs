using KeyLockIndicators.Services.ArgsReader;
using KeyLockIndicators.Services.Ipc;
using KeyLockIndicators.Services.ToastBuilder;
using Microsoft.Win32;
using System;
using System.Windows.Forms;

namespace KeyLockIndicators.Toaster
{

    internal sealed class ToastNotification : ApplicationContext
    {

        public ToastNotification(IIpcClient ipcClient, IToastBuilder toastBuilder, IArgsReader argsReader)
        {
            SystemEvents.SessionEnding += OnCloseEvent;
            ipcClient.CloseEvent += OnCloseEvent;
            toastBuilder.CloseEvent += OnCloseEvent;
            toastBuilder.Show(argsReader.ToastType);
        }


        protected override void Dispose(bool disposing)
        {
            SystemEvents.SessionEnding -= OnCloseEvent;
            base.Dispose(disposing);
        }


        private void OnCloseEvent(object sender, EventArgs e)
        {
            ExitThread();
        }

    }

}
