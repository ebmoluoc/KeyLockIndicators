using KeyLockIndicators.Interops;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.ServiceProcess;
using System.Threading;

namespace KeyLockIndicators
{

    public static class Helpers
    {

        public static bool CreateOwnedMutex(string name, out Mutex mutex)
        {
            mutex = new Mutex(true, name, out var createdNew);
            if (!createdNew)
            {
                mutex.Dispose();
                mutex = null;
            }

            return mutex != null;
        }


        public static bool GetCapsLockState()
        {
            return (NativeMethods.GetKeyState(NativeMethods.VK_CAPITAL) & 1) == 1;
        }


        public static bool GetNumLockState()
        {
            return (NativeMethods.GetKeyState(NativeMethods.VK_NUMLOCK) & 1) == 1;
        }


        public static bool GetScrollLockState()
        {
            return (NativeMethods.GetKeyState(NativeMethods.VK_SCROLL) & 1) == 1;
        }


        public static bool IsServiceRunning(string name)
        {
            using var service = new ServiceController(name);
            return service.Status == ServiceControllerStatus.Running;
        }


        public static bool IsUserLogged(int sessionId)
        {
            var success = NativeMethods.WTSQueryUserToken(sessionId, out var tokenHandle);
            if (success)
                NativeMethods.CloseHandle(tokenHandle);

            return success;
        }


        public static IntPtr GetCurrentModuleHandle()
        {
            using var currentProcess = Process.GetCurrentProcess();
            using var mainModule = currentProcess.MainModule;

            return mainModule.BaseAddress;
        }


        public static int GetActiveSessionId()
        {
            return NativeMethods.WTSGetActiveConsoleSessionId();
        }


        public static void CreateProcessAsUser(string applicationPath, string applicationArgs, int sessionId)
        {
            if (!NativeMethods.WTSQueryUserToken(sessionId, out var tokenHandle))
                throw new Win32Exception(Marshal.GetLastWin32Error());

            try
            {
                var commandLine = $"\"{applicationPath}\" {applicationArgs}";
                var startupInfo = new STARTUPINFO { cb = Marshal.SizeOf(typeof(STARTUPINFO)), lpDesktop = "winsta0\\default" };
                if (!NativeMethods.CreateProcessAsUser(tokenHandle, null, commandLine, IntPtr.Zero, IntPtr.Zero, false, NativeMethods.NORMAL_PRIORITY_CLASS, IntPtr.Zero, null, ref startupInfo, out var processInfo))
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                NativeMethods.CloseHandle(processInfo.hThread);
                NativeMethods.CloseHandle(processInfo.hProcess);
            }
            finally
            {
                if (tokenHandle != IntPtr.Zero)
                    NativeMethods.CloseHandle(tokenHandle);
            }
        }


        public static void CreateProcessAsSystem(string applicationPath, string applicationArgs, int sessionId)
        {
            var processHandle = IntPtr.Zero;
            var tokenHandle = IntPtr.Zero;

            try
            {
                processHandle = GetWinlogonProcessHandle(sessionId);
                tokenHandle = DuplicateProcessToken(processHandle);

                var commandLine = $"\"{applicationPath}\" {applicationArgs}";
                var startupInfo = new STARTUPINFO { cb = Marshal.SizeOf(typeof(STARTUPINFO)), lpDesktop = "winsta0\\default" };
                if (!NativeMethods.CreateProcessAsUser(tokenHandle, null, commandLine, IntPtr.Zero, IntPtr.Zero, false, NativeMethods.NORMAL_PRIORITY_CLASS, IntPtr.Zero, null, ref startupInfo, out var processInfo))
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                NativeMethods.CloseHandle(processInfo.hThread);
                NativeMethods.CloseHandle(processInfo.hProcess);
            }
            finally
            {
                if (processHandle != IntPtr.Zero)
                    NativeMethods.CloseHandle(processHandle);

                if (tokenHandle != IntPtr.Zero)
                    NativeMethods.CloseHandle(tokenHandle);
            }
        }


        public static bool IsCurrentProcessAsSystem()
        {
            if (!NativeMethods.OpenProcessToken(NativeMethods.GetCurrentProcess(), NativeMethods.TOKEN_QUERY, out var tokenHandle))
                throw new Win32Exception(Marshal.GetLastWin32Error());

            try
            {
                using var windowsIdentity = new WindowsIdentity(tokenHandle);
                return windowsIdentity.IsSystem;
            }
            finally
            {
                NativeMethods.CloseHandle(tokenHandle);
            }
        }


        private static IntPtr DuplicateProcessToken(IntPtr processHandle)
        {
            if (!NativeMethods.OpenProcessToken(processHandle, NativeMethods.TOKEN_DUPLICATE, out var tokenHandle))
                throw new Win32Exception(Marshal.GetLastWin32Error());

            try
            {
                if (!NativeMethods.DuplicateTokenEx(tokenHandle, NativeMethods.MAXIMUM_ALLOWED, IntPtr.Zero, SECURITY_IMPERSONATION_LEVEL.SecurityIdentification, TOKEN_TYPE.TokenPrimary, out var newTokenHandle))
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                return newTokenHandle;
            }
            finally
            {
                NativeMethods.CloseHandle(tokenHandle);
            }
        }


        private static IntPtr GetWinlogonProcessHandle(int sessionId)
        {
            var processes = Process.GetProcessesByName("winlogon");

            try
            {
                foreach (var process in processes)
                {
                    if (process.SessionId == sessionId)
                    {
                        var handle = NativeMethods.OpenProcess(NativeMethods.PROCESS_QUERY_INFORMATION, false, process.Id);
                        return handle != IntPtr.Zero ? handle : throw new Win32Exception(Marshal.GetLastWin32Error());
                    }
                }

                throw new ArgumentException("No Winlogon process was found for the specified session ID.", nameof(sessionId));
            }
            finally
            {
                foreach (var process in processes)
                    process.Dispose();
            }
        }

    }

}
