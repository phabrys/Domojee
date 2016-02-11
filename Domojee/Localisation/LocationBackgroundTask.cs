using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Geolocation;
using Windows.Storage;
using System.Linq;

namespace Localisation
{
    public sealed class LocationBackgroundTask : IBackgroundTask
    {
        private CancellationTokenSource _cts = null;
        Update UpdateInforamtion = new Update();
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            try
            {
                // Associate a cancellation handler with the background task.
                taskInstance.Canceled += OnCanceled;

                // Get cancellation token
                if (_cts == null)
                {
                    _cts = new CancellationTokenSource();
                }
                CancellationToken token = _cts.Token;

                // Create geolocator object
                Geolocator geolocator = new Geolocator();

                // Make the request for the current position
                Geoposition pos = await geolocator.GetGeopositionAsync().AsTask(token);

                DateTime currentTime = DateTime.Now;

                UpdateInforamtion.WriteStatusToAppData("Time: " + currentTime.ToString());
                await UpdateInforamtion.WriteGeolocToAppData(pos);
            }
            catch (UnauthorizedAccessException)
            {
                UpdateInforamtion.WriteStatusToAppData("Disabled");
                UpdateInforamtion.WipeGeolocDataFromAppData();
            }
            catch (Exception ex)
            {
                UpdateInforamtion.WriteStatusToAppData(ex.ToString());
                UpdateInforamtion.WipeGeolocDataFromAppData();
            }
            finally
            {
                _cts = null;
                deferral.Complete();
            }
        }

       
        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts = null;
            }
        }

        }
}