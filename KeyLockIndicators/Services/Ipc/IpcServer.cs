using KeyLockIndicators.Services.ToastBuilder;
using System;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;

namespace KeyLockIndicators.Services.Ipc
{

    public sealed class IpcServer : IIpcServer, IDisposable
    {

        private readonly EventWaitHandle _eventWaitHandle;
        private bool _notifierOpened;


        public IpcServer()
        {
            _eventWaitHandle = new(false, EventResetMode.ManualReset, AppSettings.IpcEventWaitName);

            var eventWaitHandleSecurity = _eventWaitHandle.GetAccessControl();
            var securityIdentifier = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
            var eventWaitHandleAccessRule = new EventWaitHandleAccessRule(securityIdentifier, EventWaitHandleRights.Synchronize | EventWaitHandleRights.Modify, AccessControlType.Allow);
            eventWaitHandleSecurity.AddAccessRule(eventWaitHandleAccessRule);

            _eventWaitHandle.SetAccessControl(eventWaitHandleSecurity);
        }


        public void Dispose()
        {
            _eventWaitHandle.Close();
        }


        public void OpenNotifier(int sessionId)
        {
            if (_notifierOpened)
                throw new InvalidOperationException("Notifier already opened.");

            _eventWaitHandle.Reset();
            Helpers.CreateProcessAsSystem(AppSettings.NotifierPath, null, sessionId);
            _notifierOpened = true;
        }


        public void OpenToaster(int sessionId, ToastType type)
        {
            _eventWaitHandle.Reset();
            Helpers.CreateProcessAsUser(AppSettings.ToasterPath, $"{AppSettings.ToastSwitch}{(int)type}", sessionId);
        }


        public void CloseClients()
        {
            _eventWaitHandle.Set();
            _notifierOpened = false;
        }

    }

}
