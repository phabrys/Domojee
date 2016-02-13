using Domojee.Services.SettingsServices;
using Domojee.Views;
using Jeedom;
using Jeedom.Model;
using System;
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
        private delegate Task<Jeedom.Model.Error> ActionFunction();

        private struct ActionItem
        {
            public string message;
            public ActionFunction function;
        }

        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);
            InitializeComponent();
            SplashFactory = (e) => new Views.Splash(e);

            #region App settings

            var _settings = SettingsService.Instance;
            RequestedTheme = _settings.AppTheme;
            CacheMaxDuration = _settings.CacheMaxDuration;
            ShowShellBackButton = _settings.UseShellBackButton;

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

        private async Task<bool> Action(ActionItem item)
        {
            //tbInformation.Text = item.message;
            Error error = await item.function();
            if (error != null)
            {
                //tbInformation.Text = error.message;
                await Task.Delay(new TimeSpan(0, 0, 3));
                NavigationService.Navigate(typeof(ConnectPage));
                return false;
            }
            else
                return true;
        }

        // runs only when not restored from state
        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            await Task.Delay(TimeSpan.FromSeconds(5));
            ConfigurationViewModel config = new ConfigurationViewModel();
            if (config.Populated)
            {
                ActionItem[] Actions = new ActionItem[] {
                    new ActionItem { message = "Objets",        function = RequestViewModel.GetInstance().DownloadObjects },
                    //new ActionItem { message = "Equipements",   function = RequestViewModel.GetInstance().DownloadEqLogics },
                    //new ActionItem { message = "Commandes",     function = RequestViewModel.GetInstance().DownloadCommands },
                    new ActionItem { message = "Scénarios",     function = RequestViewModel.GetInstance().DownloadScenes },
                    new ActionItem { message = "Messages",      function = RequestViewModel.GetInstance().DownloadMessages },
                    new ActionItem { message = "Interaction",   function = RequestViewModel.GetInstance().DownloadInteraction }
                };

                foreach (ActionItem item in Actions)
                {
                    if (!await Action(item))
                        return;
                }

                await Task.Delay(TimeSpan.FromSeconds(2));
                NavigationService.Navigate(typeof(DashboardPage));
            }
            else
            {
                NavigationService.Navigate(typeof(Views.ConnectPage));
            }

            return;// Task.CompletedTask;
        }
    }
}