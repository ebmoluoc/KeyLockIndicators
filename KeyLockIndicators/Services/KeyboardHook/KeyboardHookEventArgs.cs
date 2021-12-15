using System;

namespace KeyLockIndicators.Services.KeyboardHook
{
    public sealed class KeyboardHookEventArgs : EventArgs
    {
        public KeyboardHookEventArgs(bool lockState)
        {
            LockState = lockState;
        }

        public bool LockState { get; }
    }
}
