using System;

namespace KeyLockIndicators.Services.ToastBuilder
{
    public interface IToastBuilder
    {
        void Show(ToastType toastType);
        event EventHandler CloseEvent;
    }
}
