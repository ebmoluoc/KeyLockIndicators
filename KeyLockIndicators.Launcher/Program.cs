using KeyLockIndicators.Services.Ipc;
using KeyLockIndicators.Services.SensLogon2;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace KeyLockIndicators.Launcher
{

    internal sealed class Program
    {

        public static void Main()
        {
            TaskScheduler.UnobservedTaskException += OnTaskSchedulerUnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

            Log.Logger = AppSettings.Logger;

            if (!Helpers.IsCurrentProcessAsSystem())
                throw new InvalidOperationException("The launcher service is not running as System.");

            if (Helpers.IsServiceRunning(AppSettings.ServiceName))
                throw new InvalidOperationException("The launcher service is already running.");

            using var host = CreateHost();
            host.Run();
        }


        private static void OnProcessExit(object sender, EventArgs e)
        {
            Log.CloseAndFlush();
        }


        private static IHost CreateHost()
        {
            var host = Host.CreateDefaultBuilder();

            host.ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<ISensLogon2, SensLogon2>();
                services.AddSingleton<IIpcServer, IpcServer>();
                services.AddHostedService<Worker>();
            });

            host.UseSerilog();
            host.UseWindowsService();

            return host.Build();
        }


        private static void ExceptionHandler(Exception ex, [CallerMemberName] string callerMemberName = "")
        {
            Log.Fatal(ex, "{AssemblyName} | {CallerName}", typeof(Program).Assembly.GetName().Name, callerMemberName);
            Environment.Exit(1);
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

    }

}
