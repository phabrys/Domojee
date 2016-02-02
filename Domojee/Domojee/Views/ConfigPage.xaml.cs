using System;
using Windows.Foundation.Metadata;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Geolocation;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Domojee.Views
{
    public sealed partial class ConfigPage : Page
    { 
        private const string BackgroundTaskName = "LocationBackgroundTask";
        private const string BackgroundTaskEntryPoint = "BackgroundTask.LocationBackgroundTask";
        private IBackgroundTaskRegistration _geolocTask = null;    
        public ConfigPage()
        {
            this.InitializeComponent();
            var settings = ApplicationData.Current.LocalSettings;
            ObservableCollection<Domojee.Models.Command> GeolocCmd = new ObservableCollection<Domojee.Models.Command>();
            foreach (var Equipement in Domojee.ViewModels.RequestViewModel.EqLogicList) {
                if (Equipement.eqType_name == "geoloc") {
                    GeolocCmd=Equipement.GetInformationsCmds();
                }
            }
            MobilePosition_Cmd.ItemsSource = GeolocCmd;// Domojee.ViewModels.RequestViewModel.CommandList;
            if (settings.Values["Status"] != null)
            {
                Status.Text = settings.Values["Status"].ToString();
            }

            // Extract and display location data set by the background task if not null
            MobilePosition_Latitude.Text = (settings.Values["Latitude"] == null) ? "No data" : settings.Values["Latitude"].ToString();
            MobilePosition_Longitude.Text = (settings.Values["Longitude"] == null) ? "No data" : settings.Values["Longitude"].ToString();
            MobilePosition_Accuracy.Text = (settings.Values["Accuracy"] == null) ? "No data" : settings.Values["Accuracy"].ToString();

            menu.NavigateToPage += Menu_NavigateToPage;
        }
        private void Menu_NavigateToPage(object sender, Controls.NavigateEventArgs e)
        {
            Frame.Navigate(e.Page);
        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // Loop through all background tasks to see if SampleBackgroundTaskName is already registered
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
                // Associate an event handler with the existing background task
                _geolocTask.Completed += OnCompleted;

                try
                {
                    BackgroundAccessStatus backgroundAccessStatus = BackgroundExecutionManager.GetAccessStatus();

                    switch (backgroundAccessStatus)
                    {
                        case BackgroundAccessStatus.Unspecified:
                        case BackgroundAccessStatus.Denied:
                           Status.Text = "Not able to run in background. Application must be added to the lock screen.";
                            break;

                        default:
                            Status.Text = "Background task is already registered. Waiting for next update...";
                            break;
                    }
                }
                catch (Exception ex)
                {
                }
            }
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                await Windows.UI.ViewManagement.StatusBar.GetForCurrentView().HideAsync();
            }

            if (Frame.CanGoBack)
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            else
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

            base.OnNavigatedTo(e);
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
                    Status.Text = "Access to location is denied.";
                    break;

                case GeolocationAccessStatus.Unspecified:
                    Status.Text = "Unspecificed error!";
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
        async private void active_Toggled(object sender, RoutedEventArgs e)
        {
            
            if(active.IsOn==true)
            {
                try
                {
                    // Get permission for a background task from the user. If the user has already answered once,
                    // this does nothing and the user must manually update their preference via PC Settings.
                    BackgroundAccessStatus backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
                 //if (backgroundAccessStatus == BackgroundAccessStatus.Unspecified)
                    //{ 
                    // Regardless of the answer, register the background task. If the user later adds this application
                      // to the lock screen, the background task will be ready to run.
                      // Create a new background task builder
                        BackgroundTaskBuilder geolocTaskBuilder = new BackgroundTaskBuilder();

                        geolocTaskBuilder.Name = BackgroundTaskName;
                        geolocTaskBuilder.TaskEntryPoint = BackgroundTaskEntryPoint;

                        // Create a new timer triggering at a 15 minute interval
                        var trigger = new TimeTrigger(15, false);

                        // Associate the timer trigger with the background task builder
                        geolocTaskBuilder.SetTrigger(trigger);

                        // Register the background task
                        _geolocTask = geolocTaskBuilder.Register();

                        // Associate an event handler with the new background task
                        _geolocTask.Completed += OnCompleted;

                        switch (backgroundAccessStatus)
                        {
                            case BackgroundAccessStatus.Unspecified:
                            case BackgroundAccessStatus.Denied:
                                Status.Text = "Not able to run in background. Application must be added to the lock screen.";
                                break;

                            default:
                                // BckgroundTask is allowed
                                Status.Text = "Background task registered.";

                                // Need to request access to location
                                // This must be done with the background task registeration
                                // because the background task cannot display UI.
                                RequestLocationAccess();
                                break;
                        }
                //    }
                }
                catch (Exception ex)
                {
                    Status.Text = ex.ToString();
                }
            }
            else
            {
                if (null != _geolocTask)
                {
                    _geolocTask.Unregister(true);
                    _geolocTask = null;
                }
                MobilePosition_Latitude.Text = "No data";
                MobilePosition_Longitude.Text = "No data";
                MobilePosition_Accuracy.Text = "No data";

            }
        }
        private void MobilePosition_Cmd_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var settings = ApplicationData.Current.LocalSettings;
            Domojee.Models.Command ObjectsSelect = MobilePosition_Cmd.SelectedItem as Domojee.Models.Command;
            settings.Values["GeolocObjectId"] = ObjectsSelect.id;
        }
    }
}
