using KeyLockIndicators.Interops;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace KeyLockIndicators.Services.KeyboardHook
{

    public sealed class KeyboardHook : IKeyboardHook, IDisposable
    {

        private readonly IntPtr _hookHandle;
        private bool _capsLockRepeat;
        private bool _numLockRepeat;
        private bool _scrollLockRepeat;

        public event EventHandler<KeyboardHookEventArgs> KeyboardHookCapsLockEvent;
        public event EventHandler<KeyboardHookEventArgs> KeyboardHookNumLockEvent;
        public event EventHandler<KeyboardHookEventArgs> KeyboardHookScrollLockEvent;


        public KeyboardHook()
        {
            _hookHandle = NativeMethods.SetWindowsHookEx(NativeMethods.WH_KEYBOARD_LL, LowLevelKeyboardProc, Helpers.GetCurrentModuleHandle(), 0);
            if (_hookHandle == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }


        public void Dispose()
        {
            NativeMethods.UnhookWindowsHookEx(_hookHandle);
        }


        private IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode == NativeMethods.HC_ACTION)
            {
                var vkCode = Marshal.ReadInt32(lParam);

                if (wParam == NativeMethods.WM_KEYDOWN || wParam == NativeMethods.WM_SYSKEYDOWN)
                {
                    if (vkCode == NativeMethods.VK_CAPITAL && !_capsLockRepeat)
                    {
                        _capsLockRepeat = true;
                        KeyboardHookCapsLockEvent?.Invoke(this, new KeyboardHookEventArgs(!Helpers.GetCapsLockState()));
                    }
                    else if (vkCode == NativeMethods.VK_NUMLOCK && !_numLockRepeat)
                    {
                        _numLockRepeat = true;
                        KeyboardHookNumLockEvent?.Invoke(this, new KeyboardHookEventArgs(!Helpers.GetNumLockState()));
                    }
                    else if (vkCode == NativeMethods.VK_SCROLL && !_scrollLockRepeat)
                    {
                        _scrollLockRepeat = true;
                        KeyboardHookScrollLockEvent?.Invoke(this, new KeyboardHookEventArgs(!Helpers.GetScrollLockState()));
                    }
                }
                else if (wParam == NativeMethods.WM_KEYUP || wParam == NativeMethods.WM_SYSKEYUP)
                {
                    if (vkCode == NativeMethods.VK_CAPITAL)
                        _capsLockRepeat = false;
                    else if (vkCode == NativeMethods.VK_NUMLOCK)
                        _numLockRepeat = false;
                    else if (vkCode == NativeMethods.VK_SCROLL)
                        _scrollLockRepeat = false;
                }
            }

            return NativeMethods.CallNextHookEx(_hookHandle, nCode, wParam, lParam);
        }

    }

}
