using Microsoft.Toolkit.Uwp.Notifications;
using System;

namespace KeyLockIndicators.Services.ToastBuilder
{

    public sealed class ToastBuilder : IToastBuilder, IDisposable
    {

        public event EventHandler CloseEvent;


        public ToastBuilder()
        {
            ToastNotificationManagerCompat.OnActivated += OnActivated;
        }


        public void Dispose()
        {
            ToastNotificationManagerCompat.OnActivated -= OnActivated;
        }


        public void Show(ToastType toastType)
        {
            var toastContentBuilder = new ToastContentBuilder();
            toastContentBuilder.SetBackgroundActivation();

            if (toastType == ToastType.OpenNotifierFailure)
            {
                toastContentBuilder.AddArgument("action", "dismiss");
                toastContentBuilder.AddAppLogoOverride(new Uri("file:///" + AppSettings.AppLogoPath));
                toastContentBuilder.AddText(AppSettings.NotifierTitle);
                toastContentBuilder.AddText("The notifier program has failed to be launched. No icons will be displayed when the caps, num or scroll keys are locked.");
            }

            toastContentBuilder.Show();
        }


        private void OnActivated(ToastNotificationActivatedEventArgsCompat e)
        {
            if (ToastArguments.Parse(e.Argument).Get("action") == "dismiss")
            {
                ToastNotificationManagerCompat.Uninstall();
                CloseEvent?.Invoke(this, EventArgs.Empty);
            }
        }

    }

}
