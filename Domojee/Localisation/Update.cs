using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Geolocation;
using Windows.Storage;

namespace Localisation
{
    internal class Update
    {
        private const string SampleBackgroundTaskName = "DomojeeGeofenceBackgroundTask";
        private const string SampleBackgroundTaskEntryPoint = "Localisation.GeofenceBackgroundTask";
        private const long oneHundredNanosecondsPerSecond = 10000000;    // conversion from 100 nano-second resolution to seconds

        private IBackgroundTaskRegistration _geofenceTask = null;

        public Jeedom.ConfigurationViewModel config = new Jeedom.ConfigurationViewModel();

        async public Task WriteGeolocToAppData(Geoposition pos)
        {
            var settings = ApplicationData.Current.LocalSettings;
            settings.Values["Latitude"] = pos.Coordinate.Point.Position.Latitude.ToString();
            settings.Values["Longitude"] = pos.Coordinate.Point.Position.Longitude.ToString();
            settings.Values["Accuracy"] = pos.Coordinate.Accuracy.ToString();
            await Jeedom.RequestViewModel.Instance.SendPosition(pos.Coordinate.Point.Position.Latitude.ToString().Replace(',', '.') + ',' + pos.Coordinate.Point.Position.Longitude.ToString().Replace(',', '.'));
            double HomeMobile = 0;
            var HomeObjectId = settings.Values["HomeObjectId"];
            if (HomeObjectId != null)
            {
                foreach (Jeedom.Model.Command Commande in Jeedom.RequestViewModel.Instance.CommandList.Where(w => w.id.Equals(HomeObjectId)))
                {
                    var coordonee = Commande.Value.Split(',');
                    HomeMobile = Math.Round(Distance(Convert.ToDouble(coordonee[0]), Convert.ToDouble(coordonee[1]), pos.Coordinate.Point.Position.Latitude, pos.Coordinate.Point.Position.Longitude, 'K'), 2);
                }
            }

            if (config.GeoFenceActivation && Convert.ToDouble(config.GeoFenceActivationDistance) >= HomeMobile)
            {
                RegisterGeoFence();
            }
            else
            {
                UnregisterGeoFence();
            }
        }

        private double Distance(double lat1, double lon1, double lat2, double lon2, char unit)
        {
            double theta = lon1 - lon2;
            double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
            dist = Math.Acos(dist);
            dist = rad2deg(dist);
            dist = dist * 60 * 1.1515;
            if (unit == 'K')
            {
                dist = dist * 1.609344;
            }
            else if (unit == 'N')
            {
                dist = dist * 0.8684;
            }
            return (dist);
        }

        private double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        private double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }

        public void WipeGeolocDataFromAppData()
        {
            var settings = ApplicationData.Current.LocalSettings;
            settings.Values["Latitude"] = "";
            settings.Values["Longitude"] = "";
            settings.Values["Accuracy"] = "";
        }

        public void WriteStatusToAppData(string status)
        {
            var settings = ApplicationData.Current.LocalSettings;
            settings.Values["Status"] = status;
        }

        async private void RegisterGeoFence()
        {
            try
            {
                // Get permission for a background task from the user. If the user has already answered once,
                // this does nothing and the user must manually update their preference via PC Settings.
                BackgroundAccessStatus backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();

                // Regardless of the answer, register the background task. If the user later adds this application
                // to the lock screen, the background task will be ready to run.
                // Create a new background task builder
                BackgroundTaskBuilder geofenceTaskBuilder = new BackgroundTaskBuilder();

                geofenceTaskBuilder.Name = SampleBackgroundTaskName;
                geofenceTaskBuilder.TaskEntryPoint = SampleBackgroundTaskEntryPoint;

                // Create a new location trigger
                var trigger = new LocationTrigger(LocationTriggerType.Geofence);

                // Associate the locationi trigger with the background task builder
                geofenceTaskBuilder.SetTrigger(trigger);

                // If it is important that there is user presence and/or
                // internet connection when OnCompleted is called
                // the following could be called before calling Register()
                // SystemCondition condition = new SystemCondition(SystemConditionType.UserPresent | SystemConditionType.InternetAvailable);
                // geofenceTaskBuilder.AddCondition(condition);

                // Register the background task
                _geofenceTask = geofenceTaskBuilder.Register();

                switch (backgroundAccessStatus)
                {
                    case BackgroundAccessStatus.Unspecified:
                    case BackgroundAccessStatus.Denied:
                        //  _rootPage.NotifyUser("Not able to run in background. Application must be added to the lock screen.", NotifyType.ErrorMessage);
                        break;

                    default:
                        // _rootPage.NotifyUser("Geofence background task registered.", NotifyType.StatusMessage);

                        // RequestLocationAccess();
                        break;
                }
            }
            catch (Exception ex)
            {
                //_rootPage.NotifyUser(ex.ToString(), NotifyType.ErrorMessage);
            }
        }

        private void UnregisterGeoFence()
        {
            if (null != _geofenceTask)
            {
                _geofenceTask.Unregister(true);
                _geofenceTask = null;
            }

            //_rootPage.NotifyUser("Geofence background task unregistered", NotifyType.StatusMessage);
        }
    }
}