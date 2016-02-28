using Domojee.ViewModels;
using Jeedom;
using Jeedom.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.ApplicationModel.Background;
using Windows.Devices.Geolocation;
using Windows.Networking.PushNotifications;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Domojee.Views
{
    public sealed partial class SettingsPage : Page
    {
        private const string BackgroundTaskName = "LocationBackgroundTask";
        private const string BackgroundTaskEntryPoint = "Localisation.LocationBackgroundTask";
        private IBackgroundTaskRegistration _geolocTask = null;
        public ObservableCollection<Message> MessageList = RequestViewModel.Instance.MessageList;

        private Template10.Services.SerializationService.ISerializationService _SerializationService;

        public SettingsPage()
        {
            InitializeComponent();
            _SerializationService = Template10.Services.SerializationService.SerializationService.Json; var settings = ApplicationData.Current.LocalSettings;
            ObservableCollection<Jeedom.Model.Command> GeolocCmd = new ObservableCollection<Jeedom.Model.Command>();
            foreach (var Equipement in RequestViewModel.Instance.EqLogicList.Where(w => w.eqType_name.Equals("geoloc")))
            {
                foreach (var Cmd in Equipement.GetInformationsCmds())
                    GeolocCmd.Add(Cmd);
            }
            HomePosition_Cmd.ItemsSource = GeolocCmd;
            MobilePosition_Cmd.ItemsSource = GeolocCmd;
            if (GeolocCmd.Count() == 0)
                activeLocation.IsEnabled = false;
            else
                activeLocation.IsEnabled = true;
            var GeolocObjectId = settings.Values["GeolocObjectId"];
            if (GeolocObjectId != null)
            {
                foreach (var ObjectsSelect in GeolocCmd.Where(w => w.id.Equals(GeolocObjectId)))
                {
                    MobilePosition_Cmd.SelectedItem = ObjectsSelect;
                }
            }
            var HomeObjectId = settings.Values["HomeObjectId"];
            if (HomeObjectId != null)
            {
                foreach (var ObjectsSelect in GeolocCmd.Where(w => w.id.Equals(HomeObjectId)))
                {
                    HomePosition_Cmd.SelectedItem = ObjectsSelect;
                }
            }
            var ObjetctPush = RequestViewModel.Instance.EqLogicList.Where(w => w.eqType_name.Equals("pushNotification"));
            MobileNotification.ItemsSource = ObjetctPush;
            if (ObjetctPush.Count() == 0)
                activePush.IsEnabled = false;
            else
                activePush.IsEnabled = true;
            var NotificationId = settings.Values["NotificationObjectId"];
            if (NotificationId != null)
            {
                foreach (var ObjectsSelect in RequestViewModel.Instance.EqLogicList.Where(w => w.id.Equals(NotificationId)))
                {
                    MobileNotification.SelectedItem = ObjectsSelect;
                }
            }

            if (settings.Values["Status"] != null)
            {
                Status.Text = settings.Values["Status"].ToString();
            }

            // Extract and display location data set by the background task if not null
            MobilePosition_Latitude.Text = (settings.Values["Latitude"] == null) ? "No data" : settings.Values["Latitude"].ToString();
            MobilePosition_Longitude.Text = (settings.Values["Longitude"] == null) ? "No data" : settings.Values["Longitude"].ToString();
            MobilePosition_Accuracy.Text = (settings.Values["Accuracy"] == null) ? "No data" : settings.Values["Accuracy"].ToString();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var index = _SerializationService.Deserialize<int>(e.Parameter?.ToString());
            MyPivot.SelectedIndex = index;
            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {
                if (cur.Value.Name == BackgroundTaskName)
                {
                    _geolocTask = cur.Value;
                    break;
                }
            }

            if (_geolocTask != null)
            {
                _geolocTask.Completed += OnCompleted;

                try
                {
                    BackgroundAccessStatus backgroundAccessStatus = BackgroundExecutionManager.GetAccessStatus();

                    switch (backgroundAccessStatus)
                    {
                        case BackgroundAccessStatus.Unspecified:
                        case BackgroundAccessStatus.Denied:
                            Status.Text = "Impossible de fonctionner en arrière-plan. La demande doit être ajouté à l'écran de verrouillage.";
                            break;

                        default:
                            Status.Text = "La tâche de fond est déjà enregistré. Attendez la prochaine mise à jour ...";
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Status.Text = ex.Message.ToString();
                }
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (_geolocTask != null)
            {
                // Remove the event handler
                _geolocTask.Completed -= OnCompleted;
            }
            base.OnNavigatingFrom(e);
        }

        private async void RequestLocationAccess()
        {
            // Request permission to access location
            var accessStatus = await Geolocator.RequestAccessAsync();

            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:
                    break;

                case GeolocationAccessStatus.Denied:
                    Status.Text = "L'accès au GPS est refusé.";
                    break;

                case GeolocationAccessStatus.Unspecified:
                    Status.Text = "Erreur non spécifiée!";
                    break;
            }
        }

        private async void OnCompleted(IBackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs e)
        {
            if (sender != null)
            {
                // Update the UI with progress reported by the background task
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    try
                    {
                        // If the background task threw an exception, display the exception in
                        // the error text box.
                        e.CheckResult();

                        // Update the UI with the completion status of the background task
                        // The Run method of the background task sets this status.
                        var settings = ApplicationData.Current.LocalSettings;
                        if (settings.Values["Status"] != null)
                        {
                            Status.Text = settings.Values["Status"].ToString();
                        }

                        // Extract and display location data set by the background task if not null
                        MobilePosition_Latitude.Text = (settings.Values["Latitude"] == null) ? "No data" : settings.Values["Latitude"].ToString();
                        MobilePosition_Longitude.Text = (settings.Values["Longitude"] == null) ? "No data" : settings.Values["Longitude"].ToString();
                        MobilePosition_Accuracy.Text = (settings.Values["Accuracy"] == null) ? "No data" : settings.Values["Accuracy"].ToString();
                    }
                    catch (Exception ex)
                    {
                        // The background task had an error
                        Status.Text = ex.ToString();
                    }
                });
            }
        }

        async private void activePush_Toggled(object sender, RoutedEventArgs e)
        {
            var settings = ApplicationData.Current.LocalSettings;
            if (activePush.IsOn == true && settings.Values["channelUri"] == null)
            {
                var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
                await Jeedom.RequestViewModel.Instance.SendNotificationUri(channel.Uri.ToString());
                settings.Values["channelUri"] = channel.Uri.ToString();
            }
        }

        async private void activeLocation_Toggled(object sender, RoutedEventArgs e)
        {
            if (activeLocation.IsOn == true && _geolocTask == null)
            {
                try
                {
                    BackgroundAccessStatus backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
                    BackgroundTaskBuilder geolocTaskBuilder = new BackgroundTaskBuilder();

                    geolocTaskBuilder.Name = BackgroundTaskName;
                    geolocTaskBuilder.TaskEntryPoint = BackgroundTaskEntryPoint;

                    var trigger = new TimeTrigger(15, false);

                    geolocTaskBuilder.SetTrigger(trigger);

                    _geolocTask = geolocTaskBuilder.Register();

                    _geolocTask.Completed += OnCompleted;

                    switch (backgroundAccessStatus)
                    {
                        case BackgroundAccessStatus.Unspecified:
                        case BackgroundAccessStatus.Denied:
                            Status.Text = "Impossible de fonctionner en arrière-plan. La demande doit être ajouté à l'écran de verrouillage.";
                            break;

                        default:
                            Status.Text = "Enregister.";
                            RequestLocationAccess();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Status.Text = ex.ToString();
                }
            }
            if (activeLocation.IsOn == false)
            {
                if (null != _geolocTask)
                {
                    _geolocTask.Unregister(true);
                    _geolocTask = null;
                }
                MobilePosition_Latitude.Text = "Pas dedata";
                MobilePosition_Longitude.Text = "Pas de data";
                MobilePosition_Accuracy.Text = "Pas de data";
            }
        }

        private void MobilePosition_Cmd_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MobilePosition_Cmd.SelectedItem != null)
            {
                var settings = ApplicationData.Current.LocalSettings;
                Jeedom.Model.Command ObjectsSelect = MobilePosition_Cmd.SelectedItem as Jeedom.Model.Command;
                settings.Values["GeolocObjectId"] = ObjectsSelect.id;
            }
        }

        private void HomePosition_Cmd_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (HomePosition_Cmd.SelectedItem != null)
            {
                var settings = ApplicationData.Current.LocalSettings;
                Jeedom.Model.Command ObjectsSelect = HomePosition_Cmd.SelectedItem as Jeedom.Model.Command;
                settings.Values["HomeObjectId"] = ObjectsSelect.id;
            }
        }

        private void MobileNotification_Cmd_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MobileNotification.SelectedItem != null)
            {
                var settings = ApplicationData.Current.LocalSettings;
                Jeedom.Model.EqLogic EqLogicSelect = MobileNotification.SelectedItem as Jeedom.Model.EqLogic;
                settings.Values["NotificationObjectId"] = EqLogicSelect.id;
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void RebootButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ShutdownButton_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}