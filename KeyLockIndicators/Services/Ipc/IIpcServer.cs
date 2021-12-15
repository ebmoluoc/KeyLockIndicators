using KeyLockIndicators.Services.ToastBuilder;

namespace KeyLockIndicators.Services.Ipc
{
    public interface IIpcServer
    {
        void OpenNotifier(int sessionId);
        void OpenToaster(int sessionId, ToastType type);
        void CloseClients();
    }
}
