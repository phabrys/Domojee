using System;
using Windows.Foundation.Metadata;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Geolocation;
using Windows.ApplicationModel.Background;
using Windows.Storage;

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
                  //  _rootPage.NotifyUser(ex.ToString(), NotifyType.ErrorMessage);
                }
                UpdateButtonStates(/*registered:*/ true);
            }
            else
            {
                UpdateButtonStates(/*registered:*/ false);
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
        async private void RegisterBackgroundTask(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get permission for a background task from the user. If the user has already answered once,
                // this does nothing and the user must manually update their preference via PC Settings.
                BackgroundAccessStatus backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();

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

                UpdateButtonStates(/*registered*/ true);

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
            }
            catch (Exception ex)
            {
                Status.Text = ex.ToString();
                UpdateButtonStates(/*registered:*/ false);
            }
        }

        /// <summary>
        /// Get permission for location from the user. If the user has already answered once,
        /// this does nothing and the user must manually update their preference via Settings.
        /// </summary>
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

        /// <summary>
        /// This is the click handler for the 'Unregister' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UnregisterBackgroundTask(object sender, RoutedEventArgs e)
        {
            // Unregister the background task
            if (null != _geolocTask)
            {
                _geolocTask.Unregister(true);
                _geolocTask = null;
            }

            ScenarioOutput_Latitude.Text = "No data";
            ScenarioOutput_Longitude.Text = "No data";
            ScenarioOutput_Accuracy.Text = "No data";
            UpdateButtonStates(/*registered:*/ false);
          //  _rootPage.NotifyUser("Background task unregistered", NotifyType.StatusMessage);
        }

        /// <summary>
        /// Event handle to be raised when the background task is completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                        ScenarioOutput_Latitude.Text = (settings.Values["Latitude"] == null) ? "No data" : settings.Values["Latitude"].ToString();
                        ScenarioOutput_Longitude.Text = (settings.Values["Longitude"] == null) ? "No data" : settings.Values["Longitude"].ToString();
                        ScenarioOutput_Accuracy.Text = (settings.Values["Accuracy"] == null) ? "No data" : settings.Values["Accuracy"].ToString();
                    }
                    catch (Exception ex)
                    {
                        // The background task had an error
                        Status.Text = ex.ToString();
                    }
                });
            }
        }

        /// <summary>
        /// Update the enable state of the register/unregister buttons.
        /// 
        private async void UpdateButtonStates(bool registered)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    RegisterBackgroundTaskButton.IsEnabled = !registered;
                    UnregisterBackgroundTaskButton.IsEnabled = registered;
                });
        }
    }
}
