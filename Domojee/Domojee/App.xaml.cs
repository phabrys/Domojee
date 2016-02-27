using Domojee.Services.SettingsServices;
using Domojee.Views;
using Jeedom;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace Domojee
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Template10.Common.BootStrapper
    {
        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);
            InitializeComponent();
            SplashFactory = (e) => new Views.Splash(e);

            #region App settings

            //var _settings = SettingsService.Instance;
            RequestedTheme = ApplicationTheme.Light;
            CacheMaxDuration = TimeSpan.FromDays(1);
            //ShowShellBackButton = _settings.UseShellBackButton;

            #endregion App settings
        }

        // runs even if restored from state
        public override Task OnInitializeAsync(IActivatedEventArgs args)
        {
            // content may already be shell when resuming
            if ((Window.Current.Content as Shell) == null)
            {
                // setup hamburger shell
                var nav = NavigationServiceFactory(BackButton.Attach, ExistingContent.Include);
                Window.Current.Content = new Shell(nav);
            }
            return Task.CompletedTask;
        }

        // runs only when not restored from state
        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            ConfigurationViewModel config = new ConfigurationViewModel();
            SettingsService.Instance.UseShellBackButton = true;
            if (config.Populated)
            {
                if (await RequestViewModel.Instance.PingJeedom() != null)
                {
                    NavigationService.Navigate(typeof(ConnectPage));
                    return;
                }

                //Lancer le dispatchertimer
                var _dispatcher = new DispatcherTimer();
                _dispatcher.Interval = TimeSpan.FromMinutes(1);
                _dispatcher.Tick += _dispatcher_Tick;
                _dispatcher.Start();

                await Task.Delay(TimeSpan.FromSeconds(1));
                NavigationService.Navigate(typeof(DashboardPage));

                TaskFactory factory = new TaskFactory();
                var taskFactory = new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());
                await taskFactory.StartNew(async () =>
                {
                    //Shell.SetBusy(true, "Mise à jour");
                    await RequestViewModel.Instance.FirstLaunch();
                    //Shell.SetBusy(false);
                });
            }
            else
            {
                NavigationService.Navigate(typeof(ConnectPage));
            }

            return;// Task.CompletedTask;
        }

        private async void _dispatcher_Tick(object sender, object e)
        {
            //Shell.SetBusy(true, "Mise à jour");
            await RequestViewModel.Instance.UpdateTask();
            //Shell.SetBusy(false);
        }
    }
}