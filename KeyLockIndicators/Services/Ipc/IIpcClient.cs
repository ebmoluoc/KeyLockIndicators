using System;

namespace KeyLockIndicators.Services.Ipc
{
    public interface IIpcClient
    {
        event EventHandler CloseEvent;
    }
}
