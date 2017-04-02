using Domojee.Services;
using Jeedom;
using Jeedom.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Geolocation;
using Windows.Networking.PushNotifications;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Domojee.Views
{
    public sealed partial class SettingPage : Page
    {

        public SettingPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gère le click sur le bouton "Se connecter"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Connexion_Click(object sender, RoutedEventArgs e)
        {
            // Connexion à Jeedom
            var error = await RequestViewModel.Instance.ConnectJeedomByLogin();
            if (error != null)
            {
                var dialog = new MessageDialog(error.message);
                await dialog.ShowAsync();
                return;
            }

            // Création du mobile dans le plugin
            error = await RequestViewModel.Instance.CreateEqLogicMobile();
            if (error != null)
            {
                var dialog = new MessageDialog(error.message);
                await dialog.ShowAsync();
                return;
            }

            error = await RequestViewModel.Instance.SearchConfigByKey("jeedom::url", "core");
            if (error != null)
            {
                var dialog = new MessageDialog(error.message);
                await dialog.ShowAsync();
                return;
            }
            RequestViewModel.config.HostExt = RequestViewModel.Instance.configByKey;

            // TODO: Utiliser le dispatcherHelper de uwp tk
            var taskFactory = new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());
            await taskFactory.StartNew(async () =>
            {
                await RequestViewModel.Instance.FirstLaunch();
            });

            // Aller au dashboard si tout s'est bien terminé
            NavigationService.Navigate(typeof(DashboardPage));
        }
    }
}