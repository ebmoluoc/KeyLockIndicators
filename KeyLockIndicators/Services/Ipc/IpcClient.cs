using System;
using System.Threading;

namespace KeyLockIndicators.Services.Ipc
{

    public sealed class IpcClient : IIpcClient, IDisposable
    {

        private readonly EventWaitHandle _eventWaitHandle;

        public event EventHandler CloseEvent;


        public IpcClient()
        {
            _eventWaitHandle = EventWaitHandle.OpenExisting(AppSettings.IpcEventWaitName);
            new Thread(CloseThread) { IsBackground = true }.Start();
        }


        public void Dispose()
        {
            _eventWaitHandle.Close();
        }


        private void CloseThread()
        {
            _eventWaitHandle.WaitOne();
            CloseEvent?.Invoke(this, EventArgs.Empty);
        }

    }

}
