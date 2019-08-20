#if __UAP__
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Collections;
using Windows.Management.Deployment;

namespace Wokhan.UWP.Helpers
{
    public class InterAppComm
    {
        private AppServiceConnection connection = null;

        public string PackageName;
        public string AppName;

        public InterAppComm(string PackageName, string AppName)
        {
            this.PackageName = PackageName;
            this.AppName = AppName;
        }

        public async void CheckAppPresence(Action notFoundCallback, bool launch = false)
        {
            AppListEntry launcher = null;
            Windows.ApplicationModel.Package pack = new PackageManager().FindPackageForUser("", PackageName);
            if (pack != null)
            {
                launcher = (await pack.GetAppListEntriesAsync()).FirstOrDefault(a => a.DisplayInfo.DisplayName == AppName);
            }

            if (launcher == null)
            {
                notFoundCallback?.Invoke();
            }
            else
            {
                IsActive = true;

                if (launch)
                {
                    await launcher.LaunchAsync();
                }
            }
        }

        private static BackgroundTaskDeferral appServiceDeferral = null;

        public bool IsActive { get; set; }

        public void AppServiceTriggerCallback(BackgroundActivatedEventArgs args)
        {
            var details = args.TaskInstance.TriggerDetails as AppServiceTriggerDetails;
            if (details != null)
            {
                appServiceDeferral = args.TaskInstance.GetDeferral();

                args.TaskInstance.Canceled += TaskInstance_Canceled; // Associate a cancellation handler with the background task.

                connection = details.AppServiceConnection;
                connection.RequestReceived += Connection_RequestReceived;
                connection.ServiceClosed += Connection_ServiceClosed;

                sem?.Release();
            }
        }

        private async void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            if (initiatedByMe)
            {
                await args.Request.SendResponseAsync(new ValueSet() { { "OK", "" } });
                // For next calls.
                initiatedByMe = false;
            }
        }

        private void Connection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            connection = null;
        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            connection = null;
            // Complete the service deferral.
            appServiceDeferral?.Complete();
        }

        private SemaphoreSlim sem;
        private bool initiatedByMe;

        /// <summary>
        /// Sends message to the full trust process and receives a response back
        /// </summary>
        public async Task<ValueSet> SendRequest(ValueSet valueSet, bool launchIfNotRunning = true)
        {
            if (connection == null && launchIfNotRunning)
            {
                sem = new SemaphoreSlim(0);
                initiatedByMe = true;
                CheckAppPresence(null, true);
                await sem.WaitAsync();
            }

            if (connection != null)
            {
                AppServiceResponse res = (await connection.SendMessageAsync(valueSet));

                if (res.Status == AppServiceResponseStatus.Success)
                {
                    return res.Message;
                }
            }

            return new ValueSet();
        }
    }
}
#endif
