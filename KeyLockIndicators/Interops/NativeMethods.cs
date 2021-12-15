using System;
using System.Runtime.InteropServices;

namespace KeyLockIndicators.Interops
{
    internal static class NativeMethods
    {
        public const int VK_CAPITAL = 0x14;
        public const int VK_NUMLOCK = 0x90;
        public const int VK_SCROLL = 0x91;
        public const int WH_KEYBOARD_LL = 13;
        public const int HC_ACTION = 0;
        public const uint MAXIMUM_ALLOWED = 0x02000000;
        public const uint PROCESS_QUERY_INFORMATION = 0x0400;
        public const uint NORMAL_PRIORITY_CLASS = 0x00000020;
        public const uint TOKEN_DUPLICATE = 0x0002;
        public const uint TOKEN_QUERY = 0x0008;
        public static readonly IntPtr WM_KEYDOWN = (IntPtr)0x0100;
        public static readonly IntPtr WM_KEYUP = (IntPtr)0x0101;
        public static readonly IntPtr WM_SYSKEYDOWN = (IntPtr)0x0104;
        public static readonly IntPtr WM_SYSKEYUP = (IntPtr)0x0105;

        public delegate IntPtr HOOKPROC([In] int nCode, [In] IntPtr wParam, [In] IntPtr lParam);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool OpenProcessToken([In] IntPtr ProcessHandle, [In] uint DesiredAccess, [Out] out IntPtr TokenHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool DuplicateTokenEx([In] IntPtr hExistingToken, [In] uint dwDesiredAccess, [In] IntPtr lpTokenAttributes, [In] SECURITY_IMPERSONATION_LEVEL ImpersonationLevel, [In] TOKEN_TYPE TokenType, [Out] out IntPtr phNewToken);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool CreateProcessAsUser([In] IntPtr hToken, [In] string lpApplicationName, [In] string lpCommandLine, [In] IntPtr lpProcessAttributes, [In] IntPtr lpThreadAttributes, [In] bool bInheritHandles, [In] uint dwCreationFlags, [In] IntPtr lpEnvironment, [In] string lpCurrentDirectory, [In] ref STARTUPINFO lpStartupInfo, [Out] out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern bool WTSQueryUserToken([In] int SessionId, [Out] out IntPtr phToken);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle([In] IntPtr hObject);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess([In] uint dwDesiredAccess, [In] bool bInheritHandle, [In] int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern int WTSGetActiveConsoleSessionId();

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr SetWindowsHookEx([In] int idHook, [In] HOOKPROC lpfn, [In] IntPtr hmod, [In] int dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnhookWindowsHookEx([In] IntPtr hhk);

        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx([In] IntPtr hhk, [In] int nCode, [In] IntPtr wParam, [In] IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern short GetKeyState([In] int nVirtKey);
    }
}
