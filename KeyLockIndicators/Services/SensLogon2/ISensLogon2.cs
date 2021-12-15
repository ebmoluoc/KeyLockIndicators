using System;
using System.Runtime.InteropServices;

namespace KeyLockIndicators.Services.SensLogon2
{
    [ComImport, Guid("d597bab4-5b9f-11d1-8dd2-00aa004abd5e")]
    public interface ISensLogon2
    {
        void Logon([In, MarshalAs(UnmanagedType.BStr)] string bstrUserName, [In] int dwSessionId);
        void Logoff([In, MarshalAs(UnmanagedType.BStr)] string bstrUserName, [In] int dwSessionId);
        void SessionDisconnect([In, MarshalAs(UnmanagedType.BStr)] string bstrUserName, [In] int dwSessionId);
        void SessionReconnect([In, MarshalAs(UnmanagedType.BStr)] string bstrUserName, [In] int dwSessionId);
        void PostShell([In, MarshalAs(UnmanagedType.BStr)] string bstrUserName, [In] int dwSessionId);
        event EventHandler<SensLogon2EventArgs> SensLogon2Event;
    }
}
