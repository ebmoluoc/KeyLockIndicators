using System;

namespace KeyLockIndicators.Services.KeyboardHook
{
    public interface IKeyboardHook
    {
        event EventHandler<KeyboardHookEventArgs> KeyboardHookCapsLockEvent;
        event EventHandler<KeyboardHookEventArgs> KeyboardHookNumLockEvent;
        event EventHandler<KeyboardHookEventArgs> KeyboardHookScrollLockEvent;
    }
}
