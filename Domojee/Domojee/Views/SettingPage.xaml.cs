using Domojee.Services;
using Jeedom;
using System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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

            await RequestViewModel.Instance.FirstLaunch();

            // Aller au dashboard si tout s'est bien terminé
            NavigationService.Navigate(typeof(DashboardPage));
        }
    }
}