using KeyLockIndicators.Services.ArgsReader;
using KeyLockIndicators.Services.Ipc;
using KeyLockIndicators.Services.ToastBuilder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyLockIndicators.Toaster
{

    internal static class Program
    {

        [STAThread]
        public static void Main()
        {
            ApplicationConfiguration.Initialize();
            TaskScheduler.UnobservedTaskException += OnTaskSchedulerUnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;
            Application.ThreadException += OnApplicationThreadException;
            Application.ApplicationExit += OnApplicationExit;

            Log.Logger = AppSettings.Logger;

            using var serviceProvider = CreateServiceProvider();
            Application.Run(serviceProvider.GetRequiredService<ToastNotification>());
        }


        private static void OnApplicationExit(object sender, EventArgs e)
        {
            Log.CloseAndFlush();
        }


        private static ServiceProvider CreateServiceProvider()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<IIpcClient, IpcClient>();
            serviceCollection.AddSingleton<IToastBuilder, ToastBuilder>();
            serviceCollection.AddSingleton<IArgsReader, ArgsReader>();
            serviceCollection.AddSingleton<ToastNotification>();

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
