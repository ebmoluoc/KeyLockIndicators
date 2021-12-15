using Serilog;
using Serilog.Events;
using System.IO;
using System.Reflection;

namespace KeyLockIndicators
{

    public static class AppSettings
    {

        private static string DirectoryPath => Path.GetDirectoryName(typeof(AppSettings).Assembly.Location);

        private static string ResourcesPath => Path.Combine(DirectoryPath, "Resources");

        private static string LogPath => Path.Combine(DirectoryPath, "KeyLockIndicators.log");

        internal const string IpcEventWaitName = "Global\\6D52646E-393E-49AF-856F-26A4CCA28677";

        internal const string ToastSwitch = "-t:";

        internal static string NotifierPath => Path.Combine(DirectoryPath, "KeyLockIndicators.Notifier.exe");

        internal static string NotifierTitle => Assembly.LoadFrom(NotifierPath).GetCustomAttribute<AssemblyTitleAttribute>().Title;

        internal static string ToasterPath => Path.Combine(DirectoryPath, "KeyLockIndicators.Toaster.exe");

        internal static string IconCapsLockPath => Path.Combine(ResourcesPath, "IconCapsLock.ico");

        internal static string IconNumLockPath => Path.Combine(ResourcesPath, "IconNumLock.ico");

        internal static string IconScrollLockPath => Path.Combine(ResourcesPath, "IconScrollLock.ico");

        internal static string AppLogoPath => Path.Combine(ResourcesPath, "KeyLockIndicators.png");

        public const string ServiceName = "KeyLockIndicatorsSvc";

        public static ILogger Logger => new LoggerConfiguration().MinimumLevel.Override("Microsoft", LogEventLevel.Warning).WriteTo.File(LogPath, fileSizeLimitBytes: 10000000, rollOnFileSizeLimit: true, shared: true).CreateLogger();

    }

}
