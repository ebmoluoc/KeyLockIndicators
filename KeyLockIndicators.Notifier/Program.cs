using KeyLockIndicators.Services.Ipc;
using KeyLockIndicators.Services.KeyboardHook;
using KeyLockIndicators.Services.NotifyIcons;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyLockIndicators.Notifier
{

    internal static class Program
    {

        private static Mutex _mutex;


        [STAThread]
        public static void Main()
        {
            ApplicationConfiguration.Initialize();
            TaskScheduler.UnobservedTaskException += OnTaskSchedulerUnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;
            Application.ThreadException += OnApplicationThreadException;
            Application.ApplicationExit += OnApplicationExit;

            Log.Logger = AppSettings.Logger;

            if (!Helpers.CreateOwnedMutex("DF48DF69-F743-446A-A447-DB2D7D30CDBA", out _mutex))
                throw new InvalidOperationException("The notifier program is already running.");

            if (!Helpers.IsServiceRunning(AppSettings.ServiceName))
                throw new InvalidOperationException("The launcher service is not running.");

            if (!Helpers.IsCurrentProcessAsSystem())
                throw new InvalidOperationException("The notifier process is not running as System.");

            using var serviceProvider = CreateServiceProvider();
            Application.Run(serviceProvider.GetRequiredService<TaskbarNotification>());
        }


        private static void OnApplicationExit(object sender, EventArgs e)
        {
            _mutex?.ReleaseMutex();
            _mutex?.Dispose();

            Log.CloseAndFlush();
        }


        private static ServiceProvider CreateServiceProvider()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<IIpcClient, IpcClient>();
            serviceCollection.AddSingleton<INotifyIcons, NotifyIcons>();
            serviceCollection.AddSingleton<IKeyboardHook, KeyboardHook>();
            serviceCollection.AddSingleton<TaskbarNotification>();

            return serviceCollection.BuildServiceProvider();
        }


        private static void ExceptionHandler(Exception ex, [CallerMemberName] string callerMemberName = "")
        {
            var assembly = typeof(Program).Assembly;
            var message = $"An unexpected error has occurred.\n\n{ex?.Message}";

            Log.Fatal(ex, "{AssemblyName} | {CallerName}", assembly.GetName().Name, callerMemberName);
            MessageBox.Show(message, assembly.GetCustomAttribute<AssemblyTitleAttribute>().Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            Application.Exit();
        }


        private static void OnTaskSchedulerUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            ExceptionHandler(e.Exception);
        }


        private static void OnAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ExceptionHandler(e.ExceptionObject as Exception);
        }


        private static void OnApplicationThreadException(object sender, ThreadExceptionEventArgs e)
        {
            ExceptionHandler(e.Exception);
        }

    }

}
