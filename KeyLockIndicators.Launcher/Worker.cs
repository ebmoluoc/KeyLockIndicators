using KeyLockIndicators.Services.Ipc;
using KeyLockIndicators.Services.SensLogon2;
using KeyLockIndicators.Services.ToastBuilder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KeyLockIndicators.Launcher
{

    internal sealed class Worker : BackgroundService
    {

        private readonly ILogger<Worker> _logger;
        private readonly IIpcServer _ipcServer;


        public Worker(IHostApplicationLifetime appLifetime, ISensLogon2 sensLogon2, IIpcServer ipcServer, ILogger<Worker> logger)
        {
            _logger = logger;
            _ipcServer = ipcServer;

            sensLogon2.SensLogon2Event += OnSensLogon2Event;

            appLifetime.ApplicationStarted.Register(OnApplicationStarted);
            appLifetime.ApplicationStopped.Register(OnApplicationStopped);
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }


        private void OnApplicationStarted()
        {
            var sessionId = Helpers.GetActiveSessionId();

            if (Helpers.IsUserLogged(sessionId))
                OpenNotifier(sessionId);
        }


        private void OnApplicationStopped()
        {
            CloseClients();
        }


        private void OnSensLogon2Event(object sender, SensLogon2EventArgs e)
        {
            switch (e.EventType)
            {
                case SensLogon2EventType.Logon:
                    OpenNotifier(e.SessionId);
                    break;
                case SensLogon2EventType.SessionReconnect:
                    OpenNotifier(e.SessionId);
                    break;
                case SensLogon2EventType.Logoff:
                    CloseClients();
                    break;
                case SensLogon2EventType.SessionDisconnect:
                    CloseClients();
                    break;
            }
        }


        private void OpenNotifier(int sessionId)
        {
            try
            {
                _ipcServer.OpenNotifier(sessionId);
            }
            catch (Exception ex1)
            {
                _logger.LogError(ex1, "IPC server failed to open the Notifier");
                try
                {
                    _ipcServer.OpenToaster(sessionId, ToastType.OpenNotifierFailure);
                }
                catch (Exception ex2)
                {
                    _logger.LogError(ex2, "IPC server failed to open the Toaster");
                }
            }
        }


        private void CloseClients()
        {
            try
            {
                _ipcServer.CloseClients();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "IPC server failed to close the clients");
            }
        }

    }

}
