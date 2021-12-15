using System;
using System.Drawing;
using System.Windows.Forms;

namespace KeyLockIndicators.Services.NotifyIcons
{

    public sealed class NotifyIcons : INotifyIcons, IDisposable
    {

        private readonly NotifyIcon _notifyIconCapsLock = new() { Icon = new Icon(AppSettings.IconCapsLockPath) };
        private readonly NotifyIcon _notifyIconNumLock = new() { Icon = new Icon(AppSettings.IconNumLockPath) };
        private readonly NotifyIcon _notifyIconScrollLock = new() { Icon = new Icon(AppSettings.IconScrollLockPath) };


        public void Dispose()
        {
            _notifyIconCapsLock.Icon.Dispose();
            _notifyIconNumLock.Icon.Dispose();
            _notifyIconScrollLock.Icon.Dispose();

            _notifyIconCapsLock.Dispose();
            _notifyIconNumLock.Dispose();
            _notifyIconScrollLock.Dispose();
        }


        public void ShowCapsLockIcon(bool visible)
        {
            _notifyIconCapsLock.Visible = visible;
        }


        public void ShowNumLockIcon(bool visible)
        {
            _notifyIconNumLock.Visible = visible;
        }


        public void ShowScrollLockIcon(bool visible)
        {
            _notifyIconScrollLock.Visible = visible;
        }

    }

}
