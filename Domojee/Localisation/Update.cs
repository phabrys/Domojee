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
                    HomeMobile = Math.Round(Distance(Convert.ToDouble(coordonee[0].Replace('.', ',')), Convert.ToDouble(coordonee[1].Replace('.', ',')), pos.Coordinate.Point.Position.Latitude, pos.Coordinate.Point.Position.Longitude, 'K'), 2);
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
                BackgroundAccessStatus backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
                
                BackgroundTaskBuilder geofenceTaskBuilder = new BackgroundTaskBuilder();

                geofenceTaskBuilder.Name = SampleBackgroundTaskName;
                geofenceTaskBuilder.TaskEntryPoint = SampleBackgroundTaskEntryPoint;
                var trigger = new LocationTrigger(LocationTriggerType.Geofence);
                
                geofenceTaskBuilder.SetTrigger(trigger);
                _geofenceTask = geofenceTaskBuilder.Register();
                
            }
            catch
            {
            }
        }

        private void UnregisterGeoFence()
        {
            if (null != _geofenceTask)
            {
                _geofenceTask.Unregister(true);
                _geofenceTask = null;
            }
        }
    }
}