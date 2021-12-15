namespace KeyLockIndicators.Services.NotifyIcons
{
    public interface INotifyIcons
    {
        void ShowCapsLockIcon(bool visible);
        void ShowNumLockIcon(bool visible);
        void ShowScrollLockIcon(bool visible);
    }
}
