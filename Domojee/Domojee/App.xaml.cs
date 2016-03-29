using Domojee.Services.SettingsServices;
using Domojee.Views;
using Jeedom;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Template10.Controls;
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
            ShowShellBackButton = true;

            #endregion App settings
        }

        // runs even if restored from state
        public override async Task OnInitializeAsync(IActivatedEventArgs args)
        {
            // content may already be shell when resuming
            if ((Window.Current.Content as ModalDialog) == null)
            {
                // setup hamburger shell inside a modal dialog
                var nav = NavigationServiceFactory(BackButton.Attach, ExistingContent.Include);
                Window.Current.Content = new ModalDialog
                {
                    DisableBackButtonWhenModal = true,
                    Content = new Shell(nav),
                    ModalContent = new Busy(),
                };
            }
            await Task.CompletedTask;
        }

        // runs only when not restored from state
        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            ConfigurationViewModel config = new ConfigurationViewModel();
            SettingsService.Instance.UseShellBackButton = true;

            // Ne rien mettre au dessus de ce code sinon Template10 fonctionne mal.
            NavigationService.Navigate(typeof(DashboardPage));

            if (config.Populated)
            {
                //Lancer le dispatchertimer
                var _dispatcher = new DispatcherTimer();
                _dispatcher.Interval = TimeSpan.FromMinutes(1);
                _dispatcher.Tick += _dispatcher_Tick;
                _dispatcher.Start();

                var taskFactory = new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());

                // Tentative de connexion à Jeedom
                if (await RequestViewModel.Instance.PingJeedom() != null)
                {
                    ConnectPage.ShowConnectDialog();
                    return;
                }

                await taskFactory.StartNew(async () =>
                {
                    await RequestViewModel.Instance.FirstLaunch();
                });
            }
            else
            {
                ConnectPage.ShowConnectDialog();
            }

            await Task.CompletedTask;
        }

        private async void _dispatcher_Tick(object sender, object e)
        {
            //Shell.SetBusy(true, "Mise à jour");
            await RequestViewModel.Instance.UpdateTask();
            //Shell.SetBusy(false);
        }
    }
}